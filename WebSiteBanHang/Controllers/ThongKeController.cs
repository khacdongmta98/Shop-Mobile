using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;
namespace WebSiteBanHang.Controllers
{
    public class ThongKeController : Controller
    {
        QuanLyBanHangModel db = new QuanLyBanHangModel();
        // GET: ThongKe
        public ActionResult Index()
        {
            ViewBag.TongDoanhThu = ThongKeDoanhThu();
            ViewBag.TongDDH = ThongKeDonHang();
            ViewBag.TongThanhVien = TongThanhVien();
            ViewBag.TongDoanhThuTheoThang = ThongKeDoanhThuTheoThang(12, 2020);
            return View();
        }

        public decimal? ThongKeDoanhThu()
        {
            decimal? TongDoanhThu =db.ChiTietDonDatHangs.Sum(n => n.DonGia * n.SoLuong);
            return TongDoanhThu;
        }

        public double ThongKeDonHang()
        {
            //Đếm đơn đặt hàng
            double sl = db.DonDatHangs.Count();
            return sl;
        }

        public double TongThanhVien()
        {
            double slTV = db.ThanhViens.Count();
            return slTV;
        }

        public decimal? ThongKeDoanhThuTheoThang(int Thang, int Nam)
        {
            var lstDDH = db.DonDatHangs.Where(n=>n.NgayDat.Value.Month==Thang && n.NgayDat.Value.Year==Nam);
            decimal? TongTien = 0;
            //Duyệt chi tiết đơn hàng theo điều kiện
            foreach(var item in lstDDH)
            {
                TongTien += item.ChiTietDonDatHangs.Sum(n => n.DonGia * n.SoLuong);
            }
            return TongTien;
        }
        [HttpPost]
        public JsonResult Thongkenam(FormCollection f)
        {
            int Thang = Convert.ToInt32(f["txtThang"].ToString());
            int Nam = Convert.ToInt32(f["txtNam"].ToString());
            decimal? tongtien = ThongKeDoanhThuTheoThang(Thang, Nam);
            return Json(tongtien.ToString());
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                    db.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}