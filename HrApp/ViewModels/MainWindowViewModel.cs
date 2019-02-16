using Prism.Commands;
using Prism.Mvvm;
using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace HrApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                SetProperty(ref _isEnabled, value);
                ExecuteDelegateCommand.RaiseCanExecuteChanged();
            }
        }

        private string _updateText;
        public string UpdateText
        {
            get { return _updateText; }
            set { SetProperty(ref _updateText, value); }
        }

        public DelegateCommand ExecuteDelegateCommand { get; private set; }

        public DelegateCommand<string> ExecuteGenericDelegateCommand { get; private set; }

        public DelegateCommand DelegateCommandObservesProperty { get; private set; }

        public DelegateCommand DelegateCommandObservesCanExecute { get; private set; }


        public MainWindowViewModel()
        {
            ExecuteDelegateCommand = new DelegateCommand(Execute, CanExecute);

            DelegateCommandObservesProperty = new DelegateCommand(Execute, CanExecute)
                .ObservesProperty(() => IsEnabled);

            DelegateCommandObservesCanExecute = new DelegateCommand(Execute)
                .ObservesCanExecute(() => IsEnabled);

            ExecuteGenericDelegateCommand = new DelegateCommand<string>(ExecuteGeneric)
                .ObservesCanExecute(() => IsEnabled);
        }

        private void Execute()
        {
            //UpdateText = $"Updated: {DateTime.Now}";

            string userId = "Ub74b88bc191588d1270f0aab50c2b233";
            string word = "Test Line message api...";
            string mystr = "https://api.line.me/v2/bot/message/push";
            string apikey = "Bearer MfrrGCq+Fj/Tk6ZFGOZmCZ/aC0qZgn+pGdy84LCU4uoFJXYk47cL1f97D88OKoI7ck+dCPiDaF7pax1ZwOuf5p2F7BNK4g9lcbPPaMt841zCkaDPs+fTDJCOKl47ObGQrLH8WsuJJhVHJADOSGnq9AdB04t89/1O/w1cDnyilFU=";
            //JSON
            JObject obj = new JObject();
            obj.Add("to", userId);
            JArray msg = new JArray();
            JObject m1 = new JObject();
            m1.Add("type", "text");
            m1.Add("text", word);
            msg.Add(m1);
            obj.Add("messages", msg);
            string obj_S = JsonConvert.SerializeObject(obj);
            //POST
            Uri myuri = new Uri(mystr);
            var data = Encoding.UTF8.GetBytes(obj_S);
            SendRequest(myuri, data, "application/json", "POST", apikey);
        }

        private void ExecuteGeneric(string parameter)
        {
            UpdateText = parameter;
        }

        private bool CanExecute()
        {
            return IsEnabled;
        }

        private string SendRequest(Uri uri, byte[] jsonDataBytes, string contentType, string method, string authorization)
        {
            WebRequest req = WebRequest.Create(uri);
            req.ContentType = contentType;
            req.Method = method;
            req.ContentLength = jsonDataBytes.Length;
            req.Headers.Add("Authorization", authorization);

            var stream = req.GetRequestStream();
            stream.Write(jsonDataBytes, 0, jsonDataBytes.Length);
            stream.Close();

            WebResponse response = req.GetResponse();
            UpdateText = ((HttpWebResponse)response).StatusDescription;
            stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string responseFromServer = reader.ReadToEnd();

            return responseFromServer;
        }
    }
}
