namespace WebSiteBanHang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BinhLuan")]
    public partial class BinhLuan
    {
        [Key]
        public int MaBL { get; set; }

        public string NoiDungBL { get; set; }

        public int? MaThanhVien { get; set; }

        public int? MaSP { get; set; }
        public DateTime? ThoiGian { get; set; }
        public int? TraLoiBinhLuan { get; set; }
        public string Ten { get; set; }


        public virtual SanPham SanPham { get; set; }

        public virtual ThanhVien ThanhVien { get; set; }
    }
}
