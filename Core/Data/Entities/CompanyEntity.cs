using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Entities {
    [Table(name: "Companies")]
    public class CompanyEntity: AuditableEntity<long> {
        [Required]
        [MaxLength(6)]
        public string AccountNumber { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public int? TotalCustomers => Customers?.Count;
     
        public virtual ICollection<CustomerEntity> Customers { get; set; }

        [ForeignKey("Address")]
        [Column("CompanyAddress_Id")]
        public long? AddressId { get; set; }
        public virtual CompanyAddressEntity Address { get; set; }


        //public int TotalCustomers { get; set; } //число клиентов по месяцам

        //Тип Клиента - отдельная сущность
        //public string Regular { get; set; } //
        //public string Ours { get; set; }
        //public double CashAccount { get; set; }
        //Указать период

        //public int Balance { get; set; } //Счета с периодом 400
        //public int Current { get; set; } //Счета открытые из раздела Баланс
        //типы инвойсов

        //сумма открытых инвойсов за указанный период - неоплаченные Balance
        //Оплаченные  - NoBalance
    }
}
