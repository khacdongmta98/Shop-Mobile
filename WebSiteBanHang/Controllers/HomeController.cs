using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc;
using System.Web.Security;
using System.Net.Mail;
using System.Net;

namespace WebSiteBanHang.Controllers
{
    public class HomeController : Controller
    {
        QuanLyBanHangModel db = new QuanLyBanHangModel();

        public ActionResult Index()
        {

            // List điện thoại mới nhất
            var lstDTM = db.SanPhams.Where(n => n.MaLoaiSP == 1 && n.Moi == 1 && n.DaXoa == false);
            ViewBag.ListDTM = lstDTM;
            // List laptop mới nhất 
            var lstLTM = db.SanPhams.Where(n => n.MaLoaiSP == 3 && n.Moi == 1 && n.DaXoa == false);
            ViewBag.ListLTM = lstLTM;
            //List Máy tính bảng mới
            var lstMTBM = db.SanPhams.Where(n => n.MaLoaiSP == 2 && n.Moi == 1 && n.DaXoa == false);
            ViewBag.ListMTBM = lstMTBM;
            return View();
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult MenuPartial()
        {
            var lstSP = db.SanPhams; 
            return PartialView(lstSP);
        }
        [HttpGet]
        public ActionResult DangKy()
        {
            ViewBag.CauHoi = new SelectList(LoadCauHoi());
            return View();
        }

        public ActionResult DangKy1()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(ThanhVien tv, FormCollection f)
        {
            //dang ki bth cho thanh khách hàng
            tv.MaLoaiTV = 2;
            ViewBag.CauHoi = new SelectList(LoadCauHoi());
            //Kiểm tra Captcha hợp lệ
            if (this.IsCaptchaValid("Captcha is not valid"))
            {
                if (ModelState.IsValid)
                {
                    ViewBag.ThongBao = "Thêm thành công";
                    db.ThanhViens.Add(tv);
                    db.SaveChanges();
                    GuiEmail("Email xác nhận", tv.Email, "nguyennam4work@gmail.com", "Namnam1702", mail);
                }
                else
                {
                    ViewBag.ThongBao = "Thêm thất bại";
                }
                
                return View();
            }
            
            ViewBag.ThongBao = "Sai mã Captcha";
            return View();
        }
        string mail = "<h2>Email của bạn vừa được đăng kí thành Email xác nhận! <br> Nhấp link này để biết thêm chi tiết<a href='http://localhost:53174/'>Link</a></h2>";
        public void GuiEmail(string Title, string ToEmail, string FromEmail, string PassWord, string Content)
        {
            SmtpClient smtp = new SmtpClient();
            try
            {
                // goi email
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.To.Add(ToEmail); // Địa chỉ nhận
                mail.From = new MailAddress(ToEmail); // Địa chửi gửi
                mail.Subject = Title;  // tiêu đề gửi
                mail.Body = Content;                 // Nội dung
                mail.IsBodyHtml = true;
                smtp.Host = "smtp.gmail.com"; // host gửi của Gmail
                smtp.Port = 587;               //port của Gmail
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential
                (FromEmail, PassWord);//Tài khoản password người gửi
                smtp.EnableSsl = true;   //kích hoạt giao tiếp an toàn SSL
                smtp.Send(mail);   //Gửi mail đi
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = "Thêm thành công nma ko gửi mail";
            }
        }

        public List<string> LoadCauHoi()
        {
            List<string> lstCauHoi = new List<string>();
            lstCauHoi.Add("Con vật mà bạn yêu thích?");
            lstCauHoi.Add("Ca sĩ mà bạn yêu thích?");
            lstCauHoi.Add("Nghề nghiệp của bạn là gì?");
            return lstCauHoi;
        }

        public ActionResult DangNhap()
        {
            return View();
        }

        //Xây dựng Action đăng nhập
        [HttpPost]
        public ActionResult DangNhap(FormCollection f)
        {
            ////Kiểm tra tên đăng nhập và mật khẩu
            //string sTaiKhoan = f["txtTaiKhoan"].ToString();
            //string sMatKhau = f["txtMatKhau"].ToString();

            //ThanhVien tv = db.ThanhViens.SingleOrDefault(n=>n.TaiKhoan==sTaiKhoan && n.MatKhau==sMatKhau);

            //if (tv != null)
            //{
            //    Session["TaiKhoan"] = tv;
            //    return Content("<script>window.location.reload()</script>");
            //}
            //return Content("Tài khoản hoặc mật khẩu không đúng!");
            string taikhoan = f["txtTaiKhoan"].ToString();
            string matkhau = f["txtMatKhau"].ToString();

            ThanhVien tv = db.ThanhViens.SingleOrDefault(n=>n.TaiKhoan==taikhoan && n.MatKhau==matkhau);
            if (tv != null)
            {
                //Láy ra List quyền của thành viên tương ứng với loại thành viên
                var lstQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV == tv.MaLoaiTV);
                //Duyệt list quyền
                string Quyen = "";
                foreach(var item in lstQuyen)
                {
                    Quyen += item.MaQuyen + ",";
                }
                // Cắt dấu ","
                Quyen = Quyen.Substring(0, Quyen.Length - 1);
                PhanQuyen(tv.TaiKhoan,Quyen);
                Session["TaiKhoan"] = tv;
                Session["LoaiTV"] = tv.MaLoaiTV;

                return Content("<script>window.location.reload()</script>");
            }
            return Content("Tài khoản hoặc mật khẩu không đúng!");

        }

        public void PhanQuyen(string TaiKhoan, string Quyen)
        {
            FormsAuthentication.Initialize();
            var ticket = new FormsAuthenticationTicket(1,
                                          TaiKhoan, //user
                                          DateTime.Now, //Thời gian bắt đầu
                                          DateTime.Now.AddHours(3), //Thời gian kết thúc
                                          false, //Ghi nhớ?
                                          Quyen, // "DangKy,QuanLyDonHang,QuanLySanPham"
                                          FormsAuthentication.FormsCookiePath);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
            if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;
            Response.Cookies.Add(cookie);
        }

        // Tạo trang ngăn chặn truy cập
        public ActionResult LoiPhanQuyen()
        {
            return View();
        }

        public ActionResult Dangxuat()
        {
            //Gán về null
            Session["TaiKhoan"] = null;
            Session["LoaiTV"] = 0;


            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}