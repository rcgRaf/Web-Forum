using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForumApp.Models
{
    public class ThreadDTO
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public int AuthorId { get; set; }

        public int Votes { get; set; }



        public DateTime CreatedDate { get; set; }


        public virtual UserDTO User { get; set; }


    }
}