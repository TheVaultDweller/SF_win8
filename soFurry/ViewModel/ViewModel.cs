using Newtonsoft.Json.Linq;
using soFurry.API;
using soFurry.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace soFurry.ViewModel
{
    class GroupViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<BaseItem> Items { get; set; }

        private ApiHandler api;
        int maxItems = 30;

        public GroupViewModel()
        {
            Refresh = new SFCommand<object>(OnRefresh);
            Items = new ObservableCollection<BaseItem>();

            api = new ApiHandler();
            api.finished += lgn_finished;

            api.fetchAPIResponse(api.submissionDownloadLink(sortBy.date, contentType.stories, viewSource.all));
        }

        void lgn_finished(ApiHandler api, response e)
        {
            JObject obj = JObject.Parse(e.Response);

            // check to see if reply is an error
            // if this is an auth'ed call multiple errors indicate the user / pass is wrong
            if (Convert.ToBoolean((String)obj["success"]))
            {
                int page = (int)obj["currentpage"] + 1;

                JArray objItems = (JArray)obj["pagecontents"][0]["items"];

                foreach (JObject objItem in objItems)
                {
                    Items.Add(objItem.ToObject<BaseItem>());
                }

                PropertyChanged(this, new PropertyChangedEventArgs("SFItemList"));

                if (Items.Count < maxItems)
                {
                    api.fetchAPIResponse(api.submissionDownloadLink(sortBy.date, contentType.stories, viewSource.all, page));
                }
            }
        }

        public SFCommand<object> Refresh { get; set; }
        void OnRefresh(object obj)
        {
            // clear the item list, and go get another one
            Items.Clear();
            PropertyChanged(this, new PropertyChangedEventArgs("SFItemList"));
            api.fetchAPIResponse(api.submissionDownloadLink(sortBy.date, contentType.stories, viewSource.all));
        }
    }


    public class SFCommand<T> : ICommand
    {
        readonly Action<T> callback;

        public SFCommand(Action<T> callback)
        {
            this.callback = callback;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (callback != null) { callback((T)parameter); }
        }
    }
}
