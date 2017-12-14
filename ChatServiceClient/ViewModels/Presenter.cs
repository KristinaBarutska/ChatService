using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Contracts;

namespace ChatServiceClient.ViewModels
{
    public class Presenter : ObservableObject
    {
        //private readonly TextConverter _textConverter = new TextConverter(s => Presenter.GetUserName()+ ": " + s);
        private string _someText;
        private readonly ObservableCollection<string> _history = new ObservableCollection<string>();

        //public static string GetMessages()
        //{
        //    ChatService.SimpleServiceClient client = new ChatService.SimpleServiceClient();
        //    var message = (client.GetMessage());
        //    return message.Result;
        //}

        public static Guid GetUserId()
        {
            ServiceReference.ChatServiceClient client = new ServiceReference.ChatServiceClient(null);
            var id = (client.Subscribe());
            return id;
        }

        public string SomeText
        {
            get { return _someText; }
            set
            {
                _someText = value;
                RaisePropertyChangedEvent("SomeText");
            }
        }

        public IEnumerable<string> History
        {
            get { return _history; }
        }

        public ICommand DisplayMessageCommand
        {
            get { return new DelegateCommand(DisplayMessage); }
        }

        private void DisplayMessage()
        {
            if (string.IsNullOrWhiteSpace(SomeText)) return;
            AddToHistory( SomeText);
            SomeText = string.Empty;
        }

        private void AddToHistory(string item)
        {
            if (!_history.Contains(item))
                _history.Add(item);
        }
    }
}
