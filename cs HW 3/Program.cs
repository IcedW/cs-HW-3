using System;
using System.Collections.Generic;
using System.Linq;

namespace cs_HW_1
{
    public class StudentManagementException : ApplicationException
    {
        public DateTime ErrorTime { get; set; }
        public StudentManagementException(string message) : base(message)
        {
            ErrorTime = DateTime.Now;
        }
    }

    public class InvalidGradeException : StudentManagementException
    {
        public int InvalidGrade { get; set; }
        public InvalidGradeException(int grade)
            : base("wrong (0 to 100)")
        {
            InvalidGrade = grade;
        }
    }

    public class StudentNotFoundException : StudentManagementException
    {
        public string StudentName { get; set; }
        public StudentNotFoundException(string name)
            : base("wrong")
        {
            StudentName = name;
        }
    }

    public class InvalidStudentDataException : StudentManagementException
    {
        public string Field { get; set; }
        public InvalidStudentDataException(string field)
            : base("student wrong: " + field)
        {
            Field = field;
        }
    }

    public class GroupManagementException : ApplicationException
    {
        public DateTime ErrorTime { get; set; }
        public GroupManagementException(string message) : base(message)
        {
            ErrorTime = DateTime.Now;
        }
    }

    public class GroupFullException : GroupManagementException
    {
        public int MaxSize { get; set; }
        public GroupFullException(int maxSize)
            : base("group full (" + maxSize + ")")
        {
            MaxSize = maxSize;
        }
    }

    public class InvalidGroupDataException : GroupManagementException
    {
        public string Field { get; set; }
        public InvalidGroupDataException(string field)
            : base("eror in group " + field)
        {
            Field = field;
        }
    }

    public class TransferFailedException : GroupManagementException
    {
        public string Reason { get; set; }
        public TransferFailedException(string reason)
            : base("eror: " + reason)
        {
            Reason = reason;
        }
    }

    class Student1
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Location { get; set; }
        public string PhoneNum { get; set; }
        public List<int> Tests { get; } = new List<int>();
        public List<int> Coursework { get; } = new List<int>();
        public List<int> Exams { get; } = new List<int>();

        public Student1() { }

        public Student1(string firstName, string middleName, string lastName, DateTime birthday, string location, string phoneNum)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new InvalidStudentDataException("FirstName");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new InvalidStudentDataException("LastName");

            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            Birthday = birthday;
            Location = location;
            PhoneNum = phoneNum;
        }

        public void AddGrade(List<int> list, int grade)
        {
            try
            {
                if (grade < 0 || grade > 100)
                    throw new InvalidGradeException(grade);
                list.Add(grade);
            }
            catch (InvalidGradeException ex)
            {
                Console.WriteLine(ex.Message + ", " + ex.ErrorTime);
            }
        }

        public double AverageGrade()
        {
            var allGrades = Tests.Concat(Coursework).Concat(Exams).ToList();
            return allGrades.Count == 0 ? 0 : allGrades.Average();
        }

        public void ShowInfo()
        {
            Console.WriteLine($"{LastName}, {FirstName} ({AverageGrade():F2})");
        }
        public override bool Equals(object obj)
        {
            if (obj is not Student1 other) return false;
            return AverageGrade() == other.AverageGrade();
        }

        public override int GetHashCode()
        {
            return AverageGrade().GetHashCode();
        }
    }

    class Group
    {
        public string GroupName { get; set; }
        public string LessonObject { get; set; }
        public int ClassId { get; set; }
        public List<Student1> Students { get; set; } = new List<Student1>();
        public int MaxSize { get; set; } = 5;

        public Group() { }

        public Group(List<Student1> students, string groupName = "Unknown", string lessonObject = "Unknown", int classId = 1)
        {
            if (students == null)
                throw new InvalidGroupDataException("students");
            Students = new List<Student1>(students);
            GroupName = groupName;
            LessonObject = lessonObject;
            ClassId = classId;
        }

        public void ShowAllStudents()
        {
            Console.WriteLine("\ngroup: " + GroupName + ", object: " + LessonObject);
            int i = 1;
            foreach (var s in Students.OrderBy(s => s.LastName))
            {
                Console.Write(i++ + ". ");
                s.ShowInfo();
            }
        }

        public void AddStudent(Student1 student)
        {
            try
            {
                if (Students.Count >= MaxSize)
                    throw new GroupFullException(MaxSize);
                Students.Add(student);
            }
            catch (GroupFullException ex)
            {
                Console.WriteLine(ex.Message + ", " + ex.ErrorTime);
            }
        }

        public void TransferStudent(Student1 student, Group targetGroup)
        {
            try
            {
                if (!Students.Contains(student))
                    throw new StudentNotFoundException(student.FirstName);

                if (targetGroup.Students.Count >= targetGroup.MaxSize)
                    throw new TransferFailedException("full");

                Students.Remove(student);
                targetGroup.AddStudent(student);
            }
            catch (StudentManagementException ex)
            {
                Console.WriteLine("student eror: " + ex.Message);
            }
            catch (GroupManagementException ex)
            {
                Console.WriteLine("group eror:" + ex.Message);
            }
        }

        public void ExpelFailed(double minAverage = 50)
        {
            Students.RemoveAll(s => s.AverageGrade() < minAverage);
        }

        public void ExpelWorst()
        {
            if (Students.Count == 0) return;
            var worst = Students.OrderBy(s => s.AverageGrade()).First();
            Students.Remove(worst);
        }

        public static bool operator ==(Group left, Group right)
        {
            if (left is null)
                return right is null;
            if (right is null)
                return false;

            return left.Students.Count == right.Students.Count;
        }

        public static bool operator !=(Group left, Group right)
        {
            return !(left == right);
        }
        public override bool Equals(object obj)
        {
            if (obj is not Group other) return false;
            return Students.Count == other.Students.Count;
        }

        public override int GetHashCode()
        {
            return Students.Count.GetHashCode();
        }
    }

    class Program
    {
        static void Main()
        {
            try
            {
                var s1 = new Student1("Alex", "middle", "lastname", new DateTime(2011, 12, 1), "place", "1234567890");
                s1.AddGrade(s1.Tests, 95);
                s1.AddGrade(s1.Tests, -10);

                var s2 = new Student1("name2", "middle", "lastname", new DateTime(2012, 5, 10), "place", "5551234");
                s2.AddGrade(s2.Tests, 40);
                s2.AddGrade(s2.Exams, 55);

                var s3 = new Student1("name3", "middle", "lastname", new DateTime(2011, 8, 20), "place", "5559876");
                s3.AddGrade(s3.Tests, 85);
                s3.AddGrade(s3.Exams, 90);

                var groupA = new Group(new List<Student1> { s1, s2, s3 }, "A1", "object", 1);
                var groupB = new Group(new List<Student1>(), "B1", "object", 2);

                groupA.ShowAllStudents();
                groupA.TransferStudent(s2, groupB);
                groupA.TransferStudent(s2, groupB);
                groupA.ShowAllStudents();
                groupB.ShowAllStudents();
            }
            catch (Exception ex)
            {
                Console.WriteLine("eror: " + ex.Message);
            }
        }
    }
}
