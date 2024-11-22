using System.Collections.Generic;
using System.Collections.ObjectModel;
using WpfApp4.Model;
using WpfApp4.MVVM;
using WpfApp4.Repository;

namespace WpfApp4.ViewModel
{
    class ContactViewModel : ViewModelBase
    {
        private readonly MainRepository repository;
        private readonly Contact contact;

        private ObservableCollection<LinkViewModel> links;

        public ContactViewModel(Contact contact, MainRepository repository)
        {
            this.repository = repository;
            this.contact = contact;

            if (this.contact?.Links != null)
            {
                this.links = new ObservableCollection<LinkViewModel>();

                foreach (var link in this.contact.Links)
                {
                    this.links.Add(new LinkViewModel(link));
                }
            }
        }

        public int Id
        {
            get => this.contact.Id;
            set 
            {
                this.contact.Id = value;
            }
        }

        public string Name
        {
            get => this.contact.Name;
            set
            {
                this.contact.Name = value;
                this.OnPropertyChanged();
            }
        }

        public string Number
        {
            get => this.contact.Number;
            set
            {
                this.contact.Number = value;
                this.OnPropertyChanged();
            }
        }

        public string Email
        {
            get => this.contact.Email;
            set
            {
                this.contact.Email = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<LinkViewModel> Links
        {
            get => this.links;
            set
            {
                this.links = value;
                this.OnPropertyChanged();
            }
        }

        public static ObservableCollection<LinkViewModel> ConvertToLinkViewModels(List<Link> links)
        {
            ObservableCollection<LinkViewModel> viewModels = new();

            foreach (Link link in links)
            {
                viewModels.Add(new LinkViewModel(link));
            }

            return viewModels;
        }

        public void RefreshLinks()
        {
            this.Links.Clear();
            this.Links = ConvertToLinkViewModels(this.repository.GetContactsLinksFromDB(this.contact));
            this.OnPropertyChanged(nameof(this.Links));
        }
    }
}