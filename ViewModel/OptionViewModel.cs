using WpfApp4.Model;
using WpfApp4.MVVM;

namespace WpfApp4.ViewModel
{
    class OptionViewModel : ViewModelBase
    {
        private Option option;

        public OptionViewModel(Option option)
        {
            this.option = option;
        }

        public int Id
        {
            get => this.option.Id;
            set
            {
                this.option.Id = value;
                this.OnPropertyChanged();
            }
        }

        public string Name
        {
            get => this.option.Name;
            set
            {
                this.option.Name = value;
                this.OnPropertyChanged();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj as OptionViewModel == null)
            {
                return false;
            }

            return ((OptionViewModel)obj).Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.Id;
        }
    }
}
