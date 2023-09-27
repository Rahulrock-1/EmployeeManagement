using EmployeeManagement.Models;
using EmployeeManagement.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace EmployeeManagement.Controllers
{
    public class EmployeeController : Controller
    {
        private EmployeeEntieis _employee = new EmployeeEntieis();
        public ActionResult Index()
        {
            IEnumerable<Employee> employee = _employee.Employees.Where(c => c.IsDeleted == false).ToList();
            ViewBag.Employee = employee;
            return View();
        }

        public JsonResult Edit(int id)
        {
            return Json(_employee.Employees.SingleOrDefault(c => c.Id == id && !c.IsDeleted), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveEmployee(EmployeeViewModel employee)
        {
            if(ModelState.IsValid)
            {
                Employee newEmployee = new Employee();
                if(employee.Id != null)
                {
                    newEmployee = _employee.Employees.SingleOrDefault(c => c.Id == employee.Id && !c.IsDeleted);
                }
                newEmployee.Name = employee.Name;
                newEmployee.Email = employee.Email;
                newEmployee.Address = employee.Address;
                newEmployee.Phone = employee.Phone;
                if (employee.Id == null)
                {
                    _employee.Employees.Add(newEmployee);
                    TempData["Success"] = "New Employee created successfully!";
                }
                else
                {
                    _employee.Entry(newEmployee).State = EntityState.Modified;
                    TempData["Success"] = "Employee updated successfully!";
                }
                _employee.SaveChanges();
            }
            return RedirectToAction("Index", "Employee");
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                Employee existingEmployee = _employee.Employees.SingleOrDefault(c => c.Id == id && !c.IsDeleted);
                if (existingEmployee != null)
                {
                    existingEmployee.IsDeleted = true;
                    _employee.Entry(existingEmployee).State = EntityState.Modified;
                    _employee.SaveChanges();

                    TempData["Success"] = "Employee deleted successfully!";
                }
            }
            return RedirectToAction("Index", "Employee");
        }
        [HttpPost]
        public ActionResult MultipleDelete(string[] employeeIds)
        {
            foreach (string employeeId in employeeIds)
            {
                Employee existingEmployee = _employee.Employees.Find(Convert.ToInt32(employeeId));
                existingEmployee.IsDeleted = true;
                _employee.Entry(existingEmployee).State = EntityState.Modified;
            }
            _employee.SaveChanges();
            return Json("All the Employee deleted successfully!");
        }
    }

}