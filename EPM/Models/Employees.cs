//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EPM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Employees
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employees()
        {
            this.EmployeeTransfers = new HashSet<EmployeeTransfers>();
            this.EmployeeTrainings = new HashSet<EmployeeTrainings>();
        }
    
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密码至少需要6个字符")]
        public string Password { get; set; }
        [Range(0, 2, ErrorMessage = "角色只能是0（用户）、1（人事专员）或2（管理员）")]
        public Nullable<int> Role { get; set; }
        public string Contact { get; set; }
        public string CurrentDepartment { get; set; }
        public Nullable<decimal> Salary { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeTransfers> EmployeeTransfers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeTrainings> EmployeeTrainings { get; set; }
    }
}
