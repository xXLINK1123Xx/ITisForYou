//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Learning_Platform.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserCours
    {
        public int CourseId { get; set; }
        public string UserId { get; set; }
        public Nullable<int> LastLesson { get; set; }
    
        public virtual AspNetUser User { get; set; }
        public virtual Course Cours { get; set; }
    }
}