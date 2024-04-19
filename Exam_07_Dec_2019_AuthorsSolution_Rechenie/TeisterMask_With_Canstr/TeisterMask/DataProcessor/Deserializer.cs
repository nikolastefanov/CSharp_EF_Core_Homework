namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;
    using System.Text;
    using TeisterMask.Data.Models;
    using System.Xml.Serialization;
    using TeisterMask.DataProcessor.ImportDto;
    using System.IO;
    using System.Globalization;
    using TeisterMask.Data.Models.Enums;
    using Newtonsoft.Json;
    using System.Linq;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            List<Project> projects = new List<Project>();

            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ImportProjectDto[]),
                new XmlRootAttribute("Projects"));

            using (StringReader stringReader = new StringReader(xmlString))
            {
                ImportProjectDto[] projectDtos =
          (ImportProjectDto[])xmlSerializer.Deserialize(stringReader);

                foreach (ImportProjectDto projectDto in projectDtos)
                {
                    if (!IsValid(projectDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Project pr = new Project
                    {
                        Name = projectDto.Name,
                        //OpenDate = projectDto.OpenDate, //projectOpenDate,
                        //DueDate =projectDto.DueDate   //projectDueDate
                    };


                    foreach (ImportProjectTaskDto taskDto in projectDto.Tasks)
                    {

                        if (!IsValid(taskDto))
                        {
                            //ако не са валидни:име,opendate,duedate;
                            //те са required и могат да се хванат с IsValid

                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        pr.Tasks.Add(new Task
                        {
                            Name = taskDto.Name,
                            //OpenDate = taskDto.OpenDate, //taskOpenDate,
                           // DueDate = taskDto.DueDate, //taskDueDate,
                            ExecutionType = (ExecutionType)taskDto.ExecutionType,
                            LabelType = (LabelType)taskDto.LabelType
                        });


                    }

                    projects.Add(pr);
                    sb.AppendLine(string.Format(SuccessfullyImportedProject
                        , pr.Name
                        , pr.Tasks.Count));

                }

                context.Projects.AddRange(projects);
                context.SaveChanges();
            }

                    return sb.ToString().TrimEnd(); ;
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportEmployeeDto[] employeeDtos =
                JsonConvert.DeserializeObject<ImportEmployeeDto[]>(jsonString);

            List<Employee> employees =
                new List<Employee>();

            foreach (ImportEmployeeDto employeeDto in employeeDtos)
            {
                if (!IsValid(employeeDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                Employee em = new Employee
                {
                    Username = employeeDto.Username,
                    Email = employeeDto.Email,
                    Phone = employeeDto.Phone
                };
                foreach (var taskId in employeeDto.Tasks.Distinct())
                {
                    Task task = context.Tasks
                        .FirstOrDefault(t => t.Id == taskId);

                    em.EmployeesTasks.Add(
                            new EmployeeTask
                            {
                                Employee = em,
                                Task = task
                            });

                }

                employees.Add(em);
                sb.AppendLine(String.Format(SuccessfullyImportedEmployee
                    , em.Username
                    , em.EmployeesTasks.Count));


            }

            context.Employees.AddRange(employees);
            //цамо добавя new EmployeesTasks

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }




    

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}