using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TeisterMask.Data.Models.Enums;

namespace TeisterMask.Data.Models
{
    public class Task
    {
        public Task()
        {
            this.EmployeesTasks = new HashSet<EmployeeTask>();
        }
        [Key]
        public int Id { get; set; }


        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string  Name { get; set; }

        public DateTime OpenDate { get; set; }

        public DateTime DueDate { get; set; }

        [Range(0,3)]
        public ExecutionType ExecutionType { get; set; }

        [Range(0,4)]
        public LabelType LabelType { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        public Project Project { get; set; }

        public  virtual ICollection<EmployeeTask> EmployeesTasks { get; set; }


    }
}
