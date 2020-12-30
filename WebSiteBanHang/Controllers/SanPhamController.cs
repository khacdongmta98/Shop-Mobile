using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;
using PagedList;
namespace WebSiteBanHang.Controllers
{
    public class SanPhamController : Controller
    {
        QuanLyBanHangModel db = new QuanLyBanHangModel();

        [ChildActionOnly]
        public ActionResult SanPhamStyle1Partial()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult SanPhamStyle1Partia2()
        {
            return PartialView();
        }

        //Xây dựng trang xem chi tiết
        public ActionResult XemChiTiet(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Nếu không thì truy xuất csdl lấy ra sản phẩm tương ứng id
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (sp == null)
            {
                return HttpNotFound();
            }

            List<BinhLuan> comments = db.BinhLuans.Where(x => x.MaSP == id).ToList();
            foreach (var item in comments)
            {
                item.Ten = db.ThanhViens.Single(x => x.MaThanhVien == item.MaThanhVien).HoTen;
            }
            ViewBag.comments = comments;
            List<int> maspbanchay = db.Database.SqlQuery<int>("exec dbo.GetTopSP").ToList();
            List<SanPham> listsp = new List<SanPham>();
            foreach (var item in maspbanchay)
            {
                listsp.Add(db.SanPhams.Single(x => x.MaSP == item));
            }
            ViewBag.spbanchay = listsp;

            return View(sp);
        }

        [HttpPost]
        public ActionResult GuiBinhLuan(BinhLuan binhluan)
        {
            binhluan.MaBL = db.BinhLuans.Count()+1;
            if (binhluan.TraLoiBinhLuan == null)
            {
                binhluan.TraLoiBinhLuan = 0;
            }
            binhluan.ThoiGian = DateTime.Now;
            db.BinhLuans.Add(binhluan);
            db.SaveChanges();

            return Redirect("/SanPham/Xemchitiet?id="+binhluan.MaSP);
        }

        public ActionResult SanPham(int? MaLoaiSP, int? MaNSX,int? page,int? gia)
        {
            // Load sản phẩm theo 2 tiêu chí là Mã loại SP và Mã nhà sản xuất ( trong bản SanPham)
            //gia 1 - duoi 1 trieu
            //gia 2 1-5 trieu
            //gia 3 tren 5 trieu
            int min,max;
            List<SanPham> lstSP; 
            if (gia == 1)
            {
                min = 0;
                max = 1000000;

            }
            else if (gia == 2)
            {
                min = 1000000;
                max = 5000000;
            }
            else if (gia == 3)
            {
                min = 5000000;
                max = 999999999;
            }
            else
            {
                min = 0;
                max = 999999999;
            }

            if (MaNSX != null && MaLoaiSP != null)
            {
                lstSP = db.SanPhams.Where(n => n.MaLoaiSP == MaLoaiSP && n.MaNSX == MaNSX &&(n.DonGia>min && n.DonGia<max)).ToList();
            }
            else if (MaNSX == null && MaLoaiSP != null)
            {
                lstSP = db.SanPhams.Where(n => n.MaLoaiSP == MaLoaiSP && (n.DonGia > min && n.DonGia < max)).ToList();

            }
            else if (MaNSX != null && MaLoaiSP == null)
            {
                lstSP = db.SanPhams.Where(n => n.MaNSX == MaNSX && (n.DonGia > min && n.DonGia < max)).ToList();

            }
            else
            {
                lstSP = db.SanPhams.Where(n => n.DonGia > min && n.DonGia < max).ToList();
            }
            //Thực hiện chức năng phân trang
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            //Tạo biến số sp trên trang
            int PageSize = 9;
            //Tạo biến thứ 2 : Số trang hiện tại
            int PageNumber = (page ?? 1);
            ViewBag.MaLoaiSP = MaLoaiSP;
            ViewBag.MaNSX = MaNSX;

            List<int> maspbanchay = db.Database.SqlQuery<int>("exec dbo.GetTopSP").ToList();
            List<SanPham> listsp = new List<SanPham>();
            foreach (var item in maspbanchay)
            {
                listsp.Add(db.SanPhams.Single(x => x.MaSP == item));
            }
            ViewBag.spbanchay = listsp;

            //return View(lstSP);
            // trả về dạng list đã sắp xếp
            return View(lstSP.OrderBy(n => n.MaSP).ToPagedList(PageNumber, PageSize));
        }
    }
}