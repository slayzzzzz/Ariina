using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ariina.Models
{
    public class Media
    {
        //File paths
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }


        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tag { get; set; }

        [Required]
        [DisplayName("Make Private")]
        public bool IsPrivate { get; set; }
        public bool Hd { get; set; }
        public bool IsBeingConverted { get; set; }
        public VideoQuality VideoQuality { get; set; }

        [Required]
        public virtual string Category { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public bool IsHd()
        {
            return VideoQuality != VideoQuality.p360;
        }

    }
}