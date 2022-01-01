using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopLearn.Data.Entities.Wallet
{
    public class Wallet
    {
        [Key]
        public int WalletId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int TypeId { get; set; }

        [Display(Name = "مبلغ تراکنش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Amount { get; set; }
        [Display(Name = "توضیح تراکنش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکتر باشد")]
        public string Description { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        public bool IsPayed { get; set; }

        #region Relations

        public User.User User { get; set; }
        [ForeignKey("TypeId")]
        public WalletType WalletType { get; set; }

        #endregion
    }
}
