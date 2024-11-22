using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WpfApp4.Model;
using WpfApp4.MVVM;
using WpfApp4.Repository;
using System.Data;

namespace WpfApp4.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private bool isAddContactSelected;
        private bool canWriteName;
        private string name;
        private string number;
        private string email;
        private string addContactText;
        private string optionName;
        private string linkName;
        private string searchedContact;
        private string placeholder;
        private ContactViewModel selectedContact;
        private OptionViewModel selectedOption;
        private ObservableCollection<ContactViewModel> shownContacts;
        
        public MainWindowViewModel()
        {
            this.Repository = new MainRepository();
            this.Repository.CreateTables();

            this.Options = new ObservableCollection<OptionViewModel>();
            this.LoadOptions();

            this.Contacts = new ObservableCollection<ContactViewModel>();
            this.LoadContacts();

            this.Placeholder = "Search";
            this.CanWriteName = false;
        }

        public RelayCommand AddContactCommand => new(execution => this.AddContact());
        public RelayCommand RemoveContactCommand => new(execution => this.RemoveContact(), canExecute => this.CanRemoveContact());
        public RelayCommand AddOptionCommand => new(execution => this.AddOption(), canExecute => this.CanAddOption());
        public RelayCommand RemoveOptionCommand => new(execution => this.RemoveOption(), canExecute => this.CanRemoveOption());
        public RelayCommand AddLinkCommand => new(execution => this.AddLink(), canExecute => this.CanAddLink());
        public RelayCommand SearchCommand => new(execution => this.SearchContacts());
        public RelayCommand SaveContactCommand => new(execution => this.SaveContact(), canExecute => this.CanSaveContact());
        public RelayCommand ClearTextCommand => new(execution => this.ClearText());
        public RelayCommand IsSearchEmptyCommand => new(execution => this.IsEmpty());

        public MainRepository Repository { get; set; }
        public ObservableCollection<ContactViewModel> Contacts { get; set; }
        public ObservableCollection<OptionViewModel> Options { get; set; }

        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                this.OnPropertyChanged();
            }
        }

        public string Number
        {
            get => this.number;
            set
            {
                this.number = value;
                this.OnPropertyChanged();
            }
        }

        public string Email
        {
            get => this.email;
            set
            {
                this.email = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsAddContactSelected
        {
            get => this.isAddContactSelected;
            set
            {
                this.isAddContactSelected = value;
            }
        }

        public string AddContactText
        {
            get => this.addContactText;
            set
            {
                this.addContactText = value;
                this.OnPropertyChanged();
            }
        }

        public string OptionName
        {
            get => this.optionName;
            set
            {
                this.optionName = value;
                this.OnPropertyChanged();
            }
        }

        public string LinkName
        {
            get => this.linkName;
            set
            {
                this.linkName = value;
                this.OnPropertyChanged();
            }
        }

        public bool CanWriteName
        {
            get => this.canWriteName;
            set
            {
                this.canWriteName = value;
                this.OnPropertyChanged();
            }
        }

        public string Placeholder
        {
            get => this.placeholder;
            set
            {
                this.placeholder = value;
                this.OnPropertyChanged();
            }
        }

        public string SearchedContact
        {
            get => this.searchedContact;
            set
            {
                this.searchedContact = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<ContactViewModel> ShownContacts
        {
            get => this.shownContacts;
            set
            {
                this.shownContacts = value;
                this.OnPropertyChanged();
            }
        }

        public ContactViewModel SelectedContact
        {
            get => this.selectedContact;
            set
            {
                this.selectedContact = value;
                if (this.SelectedContact != null)
                {
                    this.Name = this.SelectedContact.Name;
                    this.Number = this.SelectedContact.Number;
                    this.Email = this.SelectedContact.Email;
                    this.CanWriteName = true;
                }
                else
                {
                    this.Name = "";
                    this.Number = "";
                    this.Email = "";
                    this.CanWriteName = false;
                }

                this.IsAddContactSelected = false;
                this.AddContactText = "";
                this.OnPropertyChanged();
            }
        }

        public OptionViewModel SelectedOption
        {
            get => this.selectedOption;
            set
            {
                this.selectedOption = value;
                this.OnPropertyChanged();
            }
        }

        public static ObservableCollection<OptionViewModel> ConvertToOptionViewModels(List<Option> options)
        {
            ObservableCollection<OptionViewModel> viewModels = new();

            foreach (Option option in options)
            {
                viewModels.Add(new OptionViewModel(option));
            }

            return viewModels;
        }

        public void LoadContacts()
        {
            this.Contacts.Clear();
            this.Contacts = this.ShownContacts = this.ConvertToContactViewModels(MainRepository.GetContactsFromDB());
        }

        public ObservableCollection<ContactViewModel> ConvertToContactViewModels(List<Contact> contacts)
        {
            ObservableCollection<ContactViewModel> viewModels = new();

            foreach (Contact contact in contacts)
            {
                viewModels.Add(new ContactViewModel(contact, this.Repository));
            }

            return viewModels;
        }

        public void LoadOptions()
        {
            this.Options.Clear();
            this.Options = ConvertToOptionViewModels(MainRepository.GetOptionsFromDB());
            this.OnPropertyChanged(nameof(this.Options));
        }

        private void AddContact()
        {
            this.SelectedContact = null;
            this.IsAddContactSelected = true;
            this.CanWriteName = true;
            this.AddContactText = "Adding new contact:";
        }

        private void RemoveContact()
        {
            this.Repository.DeleteRow(SelectedContact.Id);
            this.Repository.RemoveLinksFromContact(SelectedContact.Id);
            this.Contacts.Remove(SelectedContact);
        }

        private bool CanRemoveContact()
        {
            if (this.SelectedContact != null){
                return true;
            }
            return false;
        }

        private void AddOption()
        {
            Option option = new Option(this.OptionName);
            this.Repository.AddOption(option);

            this.LoadOptions();

            this.LoadContacts();

            this.OptionName = "";
        }

        private bool CanAddOption()
        {
            if (!string.IsNullOrWhiteSpace(this.OptionName))
            {
                return true;
            }

            return false;
        }

        private void RemoveOption()
        {

            this.Repository.RemoveLinksFromOptions(this.SelectedOption.Id);
            this.Repository.RemoveOption(this.SelectedOption.Id);

            this.LoadOptions();

            this.LoadContacts();
        }

        private bool CanRemoveOption()
        {
            if (this.SelectedOption != null)
            {
                return true;
            }

            return false;
        }

        private void SearchContacts()
        {
            if (string.IsNullOrEmpty(this.SearchedContact))
            {
                this.ShownContacts = this.Contacts;
            }
            else
            {
                this.ShownContacts = new ObservableCollection<ContactViewModel>(this.Contacts.Where(c => c.Name.ToLower().Contains(this.SearchedContact.ToLower())));
            }
        }

        private void AddLink()
        {
            LinkViewModel linkvm = new( new Link(this.SelectedContact.Id))
            {
                IsNotAssigned = true
            };

            this.SelectedContact.Links.Add(linkvm);
        }

        private bool CanAddLink()
        {
            return this.SelectedContact != null;
        }

        private void SaveContact()
        {
            if(this.IsAddContactSelected ==  false) 
            {
                this.SelectedContact.Name = this.Name;
                this.SelectedContact.Number = this.Number;
                this.SelectedContact.Email = this.Email;

                this.Repository.UpdateRow(this.SelectedContact.Name, this.SelectedContact.Number, this.SelectedContact.Email, this.SelectedContact.Id);

                foreach (LinkViewModel link in this.SelectedContact.Links)
                {
                    if (string.IsNullOrWhiteSpace(link.Value))
                    {
                        this.Repository.RemoveLink(link.Id);
                    }
                    else
                    {
                        this.Repository.UpdateLink(link.Value, link.Id);
                    }
                }

                foreach (LinkViewModel link in this.SelectedContact.Links)
                {
                    if (link.Option != null && !string.IsNullOrWhiteSpace(link.Value) && link.IsNotAssigned == true)
                    {
                        this.Repository.AddLink(link.ContactId, link.Option.Id, link.Value);
                        link.IsNotAssigned = false;
                    }
                }

                this.SelectedContact.RefreshLinks();

                this.SelectedContact = null;
            }
            else
            {
                Contact contact = new(this.Name, this.Number, this.Email);

                this.Repository.AddRow(contact);

                this.LoadContacts();

                this.IsAddContactSelected = false;
                this.AddContactText = "";
                this.CanWriteName = false;
            }
        }
        
        private bool CanSaveContact()
        {
            return this.SelectedContact != null
                || (this.IsAddContactSelected == true
                && !string.IsNullOrWhiteSpace(this.Name)
                && !string.IsNullOrWhiteSpace(this.Number)
                && !string.IsNullOrWhiteSpace(this.Email));
        }

        public void ClearText()
        {
            this.SearchedContact = string.Empty;
        }

        public void IsEmpty()
        {
            this.Placeholder = string.IsNullOrWhiteSpace(this.SearchedContact) ? "Search" : string.Empty;
        }
    }
}