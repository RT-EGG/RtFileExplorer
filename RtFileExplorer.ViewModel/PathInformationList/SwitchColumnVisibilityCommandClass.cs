using RtFileExplorer.Model.FileInformation;
using System;

namespace RtFileExplorer.ViewModel.Wpf.PathInformationList
{
    public partial class PathInformationListViewModel
    {
        class SwitchColumnVisibilityCommandClass : CommandBase<PathInformationListViewModel>
        {
            public SwitchColumnVisibilityCommandClass(PathInformationListViewModel inViewModel) 
                : base(inViewModel)
            { }

            public override bool CanExecute(object? parameter)
                => parameter is FilePropertyItemType;

            public override void Execute(object? parameter)
            {
                if (parameter is not FilePropertyItemType)
                    throw new InvalidProgramException();

                var type = (FilePropertyItemType)parameter;
                var column = ViewModel.GetColumn(type);

                if (column is null)
                    Logger.Instance.PushLog(new Logger.LogObject
                    {
                        Level = Logger.LogLevel.Error,
                        Message = $"[{type}]カラムの取得に失敗"
                    });
                else
                    column.IsVisible = !column.IsVisible;
            }
        }
    }
}
