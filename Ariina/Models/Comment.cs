using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.DynamicData;

namespace Ariina.Models
{
    public class Comment
    {
        public Comment()
        {
            Children = new List<Comment>();
        }

        [Key]
        public int CommentId { get; set; }
        public string Text { get; set; }
        public DateTime PostedDate { get; set; }

        public string InReplyTo
        {
            get { return Parent != null && Parent.User != null ? Parent.User.UserName : null; }
        }

        public string Fullname
        {
            get { return User != null ? User.UserName : null; }
        }

        public string Picture
        {
            get { return HostingEnvironment.MapPath("~/Content/images/user_blank_picture.png"); }
        }

        public int VideoId { get; set; }
        public virtual Media Video { get; set; }
        public virtual int? ParentId { get; set; }
        public virtual Comment Parent { get; set; }
        public virtual ICollection<Comment> Children { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}