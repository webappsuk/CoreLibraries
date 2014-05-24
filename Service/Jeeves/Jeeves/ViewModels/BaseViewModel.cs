using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace Jeeves.ViewModels
{
    public abstract class BaseViewModel : ViewModelBase
    {
        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CanBeNull][CallerMemberName] string propertyName = null)
        {
            base.RaisePropertyChanged(propertyName);
        }
    }
}