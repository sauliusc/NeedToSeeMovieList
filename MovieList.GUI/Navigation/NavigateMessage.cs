using MovieList.GUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.GUI.Navigation
{
    public class NavigateMessage
    {
        #region Properties

        public MovieItemModel Context { get; set; }

        public WindowAction WindowAction { get; set; }

        #endregion Properties

        #region Constructors

        public NavigateMessage(WindowAction windowType, MovieItemModel context = null)
        {
            WindowAction = windowType;
            Context = context;
        }

        #endregion Constructors
    }
}
