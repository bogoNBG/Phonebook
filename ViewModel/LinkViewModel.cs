using WpfApp4.Model;
using WpfApp4.MVVM;
using WpfApp4.Repository;

namespace WpfApp4.ViewModel
{
    class LinkViewModel : ViewModelBase
    {
        private readonly Link link;
        private OptionViewModel option;

        public LinkViewModel(Link link)
        {
            this.link = link;

            var option = MainRepository.GetOptionByIdFromDB(this.link.OptionId);

            if (option != null)
            {
                this.Option = new OptionViewModel(option);
            }
        }

        public int Id
        {
            get => this.link.Id;
            set
            {
                this.link.Id = value;
                this.OnPropertyChanged();
            }
        }

        public int ContactId
        {
            get => this.link.ContactId;
            set
            {
                this.link.ContactId = value;
                this.OnPropertyChanged();
            }
        }

        public OptionViewModel Option
        {
            get => this.option;
            set
            {
                this.option = value;
                this.OnPropertyChanged();
            }
        }

        public string Value
        {
            get => this.link.Value;
            set
            {
                this.link.Value = value;
                this.OnPropertyChanged();                
            }
        }

        public bool IsNotAssigned
        {
            get => this.link.IsNotAssigned;
            set 
            {
                this.link.IsNotAssigned = value;
                this.OnPropertyChanged(); 
            }
        }
    }
}
