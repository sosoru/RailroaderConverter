using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml;
using Livet;
using Livet.Command;
using Livet.Messaging;
using Livet.Messaging.File;
using Livet.Messaging.Window;
using Microsoft.Win32;
using RouteVisualizer.Models;
using RouteVisualizer.Railroader;

namespace RouteVisualizer.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        /*コマンド、プロパティの定義にはそれぞれ 
         * 
         *  ldcom   : DelegateCommand(パラメータ無)
         *  ldcomn  : DelegateCommand(パラメータ無・CanExecute無)
         *  ldcomp  : DelegateCommand(型パラメータ有)
         *  ldcompn : DelegateCommand(型パラメータ有・CanExecute無)
         *  lprop   : 変更通知プロパティ
         *  
         * を使用してください。
         */

        /*ViewModelからViewを操作したい場合は、
         * Messengerプロパティからメッセージ(各種InteractionMessage)を発信してください。
         */

        /*
         * UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
         * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
         */

        /*
         * Modelからの変更通知などの各種イベントをそのままViewModelで購読する事はメモリリークの
         * 原因となりやすく推奨できません。ViewModelHelperの各静的メソッドの利用を検討してください。
         */

        public MainWindowViewModel()
        {
            try
            {
                var opendiag = new OpenFileDialog();
                opendiag.Title = "open railroader file";
                if (!((bool)opendiag.ShowDialog()))
                {
                    App.Current.Shutdown(-1);
                }

                var map = new RailroaderIO.RailroaderMap(opendiag.FileName);
                var layout = map.ToLayout();
                this.LayoutVm = new LayoutViewModel(layout) { DrawingSize = new System.Windows.Size(1000, 1500) };

                var savepath = opendiag.FileName + DateTime.Now.ToString().Replace(':', '-').Replace('/', '-') + ".svg";
                DumpSvg(savepath, this.LayoutVm);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void DumpSvg(string path, LayoutViewModel layout)
        {
            using (var xw = new XmlTextWriter(path, Encoding.UTF8))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("svg");
                xw.WriteAttributeString("viewBox", "0 0 2000 2000");
                foreach (var r in layout.Rails)
                {
                    xw.WriteStartElement("g");
                    r.WriteSvgGeometry(xw);
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
                xw.WriteEndDocument();
            }
        }

        public LayoutViewModel LayoutVm
        {
            get;
            private set;
        }

    }
}
