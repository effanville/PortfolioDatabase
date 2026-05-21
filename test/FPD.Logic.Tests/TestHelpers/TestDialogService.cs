using System;

using Effanville.Common.UI.Services;

namespace Effanville.FPD.Logic.Tests.TestHelpers
{
    public class TestDialogService : IBaseDialogCreationService
    {
        private Guid Id = Guid.NewGuid();
        private Action<object> _customDialogAction;
        public void SetupCustomDialogAction(Action<object> action) => _customDialogAction = action;
        public void DisplayCustomDialog(object obj) => _customDialogAction(obj);
        public MessageBoxOutcome ShowMessageBox(string text, string title, BoxButton buttons, BoxImage imageType)
            => throw new NotImplementedException();
    }
}
