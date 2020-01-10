﻿using mvctk.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
namespace mvctk.Controllers

{
    public class AdminController : Controller
    {
        private SQLContext DB = null;

        public AdminController()
        {
            DB = new SQLContext();
        }

        public ActionResult Admin(user user)
        {
            Session["user"] = user.ID;
            return View(user);
        }

        public ActionResult AddCourse()
        {
            //  Session["lecturers"] = DB.users;
            return View();
        }

        [HttpPost]
        public ActionResult Submit(course model)
        {
            if (ModelState.IsValid)
            {
                if (model != null)
                {

                    int flag = 0;
                    var cors = DB.courses.FirstOrDefault(s => s.ID.Equals(model.ID));
                    foreach (user u in DB.users)
                        if (model.LecturerID.Equals(u.ID) && u.UserTyper == 1)
                            flag = 1;
                    if (cors == null)
                    {
                        if (flag == 1)
                        {
                            DB.courses.Add(model);

                            DB.SaveChanges();

                            return RedirectToAction("Index");
                        }
                        else
                            ModelState.AddModelError("LecturerID", "The Lecturer does not exsit!");
                    }
                    else
                        ModelState.AddModelError("ID", "The Course Already exsit!");

                }
            }

            return View("AddCourse", model);
        }

        public ActionResult Index()
        {
            return View(DB.courses);
        }

        public ActionResult Delete(String id)
        {
            Session["ID"] = id;
            if (id == null)

            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            course course = DB.courses.Find(id);

            if (course == null)
            {

                return HttpNotFound();

            }

            return View(course);

        }



        [HttpPost, ActionName("Delete")]

        public ActionResult DelteConfirmed(string id)

        {

            course course = DB.courses.Find(id);

            DB.courses.Remove(course);

            DB.SaveChanges();

            return RedirectToAction("Index");

        }

        public ActionResult Edit(string id)
        {

            if (id == null)

            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            course course = DB.courses.Find(id);

            if (course == null)

            {

                return HttpNotFound();

            }

            return View(course);

        }

        [HttpPost]
        public ActionResult Edit(course course)
        {
            if (ModelState.IsValid)
            {
                course temp2 = null;
                int flag = 0;


                List<string> user_courses = new List<string>();
                List<string> courses = new List<string>();
                foreach (grade g in DB.grades)
                    if (g.CourseID.Equals(course.ID))
                        user_courses.Add(g.StudentID);
                foreach (string s in user_courses)

                    foreach (grade g in DB.grades)
                        if (g.StudentID.Equals(s))
                            if (!g.CourseID.Equals(course.ID))
                                courses.Add(g.CourseID);


                foreach (string s in courses)
                    foreach (course c in DB.courses)
                        if (s.Equals(c.ID))
                            if (c.startlec.Equals(course.startlec) && c.Day.Equals(course.Day))
                                flag = 1;



                if (course != null && flag == 0)
                {

                    DB.Entry(course).State = EntityState.Modified;
                    DB.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                    ModelState.AddModelError("Time", "There are conflict with onther course at the same time");

            }
            return View(course);
        }


        public ActionResult EditExam(string id)
        {

            if (id == null)

            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            course course = DB.courses.Find(id);

            if (course == null)

            {

                return HttpNotFound();

            }

            return View(course);

        }

        [HttpPost]
        public ActionResult EditExam(course course)
        {
            if (ModelState.IsValid)

            {

                DB.Entry(course).State = EntityState.Modified;

                DB.SaveChanges();

                return RedirectToAction("Index");

            }

            return View(course);

        }

    }
}