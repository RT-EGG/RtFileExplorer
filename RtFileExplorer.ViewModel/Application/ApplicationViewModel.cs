using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtFileExplorer.ViewModel.Wpf.Application
{
    using ApplicationModel = Model.Application.Application;
    public class ApplicationViewModel : ViewModelBase<ApplicationModel>
    {
        public ApplicationViewModel()
        {
            BackgroundTaskQueue.Instance.Start();
        }
    }
}
