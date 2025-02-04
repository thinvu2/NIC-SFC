using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;

namespace PACK_PALT.ViewModel
{
    public class ControlBarViewModel : BaseViewModel 
    {
        #region commands
        public ICommand CloseWindowCommand { get; set; }
        public ICommand MiniMizeWindowCommand { get; set; }
        public ICommand MaxiMizeWindowCommand { get; set; }
        public ICommand MouseMoveWindowCommand { get; set; }
        #endregion

        public ControlBarViewModel ()
        {
            CloseWindowCommand = new RelayCommand<UserControl>((p) => { return p==null? false:true ; } ,(p)=> { FrameworkElement window = GetWindowParent(p);
            var w = window as Window ;
                if (w != null)
                {
                    w.Close();
                }
            }
            );

            MiniMizeWindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) => {
                FrameworkElement window = GetWindowParent(p);
                var w = window as Window;
                if (w != null)
                {
                    if (w.WindowState != WindowState.Minimized)
                        w.WindowState = WindowState.Minimized;
                    else
                        w.WindowState = WindowState.Normal;
                }
            }
            );

            MaxiMizeWindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) => {
                FrameworkElement window = GetWindowParent(p);
                var w = window as Window;
                if (w != null)
                {
                    if (w.WindowState != WindowState.Maximized)
                        w.WindowState = WindowState.Maximized ;
                    else
                        w.WindowState = WindowState.Normal;
                      
                }
            }
            );

            MouseMoveWindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) => {
                FrameworkElement window = GetWindowParent(p);
                var w = window as Window;
                if (w != null)
                {
                    w.DragMove();
                }
            }
);
        }


        FrameworkElement GetWindowParent(UserControl p)
        {
            FrameworkElement Parent = p;
            while (Parent.Parent != null  )
            {
                Parent = Parent.Parent as FrameworkElement;
            }
            return Parent;
        }
    }
}
