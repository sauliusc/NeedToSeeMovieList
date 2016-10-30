using MovieList.Core.SharedObjects;
using PostSharp.Patterns.Contracts;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.GUI.Models
{
    [NotifyPropertyChanged]
    public class MovieItemModel
    {
        #region Properties

        public DateTime CreateDate { get; set; }

        public string DescriptionUrl { get; set; }

        public DateTime? Duration { get; set; }

        public string FileUrl { get; set; }

        public string Guid { get; set; }

        public int Priority { get; set; }

        public int Ratio { get; set; }

        public bool Seen { get; set; }

        public DateTime? SeenTime { get; set; }

        [Required]
        public MovieTypes Type { get; set; }

        [Required]
        public string Title { get; set; }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return string.Format("{0} ({1:HH:MM}) - {2}", Title, Duration, Priority);
        }

        #endregion Methods
    }
}
