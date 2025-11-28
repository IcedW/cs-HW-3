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
        private string name;
        private string lastname;
        private int age;
        private double averageGrade;

        public event Action LectureMissed;
        public event Action AutomatReceived;
        public event Action ScholarshipAwarded;


        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidStudentDataException("Name");
                name = value;
            }
        }

        public string Lastname
        {
            get => lastname;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidStudentDataException("Lastname");
                lastname = value;
            }
        }

        public int Age
        {
            get => age;
            set
            {
                if (value < 0 || value > 120)
                    throw new InvalidStudentDataException("Age");
                age = value;
            }
        }

        public double AverageGrade
        {
            get => averageGrade;
            set
            {
                if (value < 0 || value > 100)
                    throw new InvalidGradeException((int)value);
                averageGrade = value;
            }
        }

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
                if (grade == 100)
                    AutomatReceived?.Invoke();
            }
            catch (InvalidGradeException ex)
            {
                Console.WriteLine(ex.Message + ", " + ex.ErrorTime);
            }
        }

        public double AverageGradeCalc()
        {
            var allGrades = Tests.Concat(Coursework).Concat(Exams).ToList();
            double avg = allGrades.Count == 0 ? 0 : allGrades.Average();
            if (avg >= 10)
                ScholarshipAwarded?.Invoke();
            return avg;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"{LastName}, {FirstName} ({AverageGradeCalc():F2})");
        }
        public override bool Equals(object obj)
        {
            if (obj is not Student1 other) return false;
            return AverageGradeCalc() == other.AverageGradeCalc();
        }

        public override int GetHashCode()
        {
            return AverageGradeCalc().GetHashCode();
        }

        public void CheckTime()
        {
            var now = DateTime.Now;
            var lectureStart = DateTime.Today.AddHours(16).AddMinutes(45);

            if (now > lectureStart)
                LectureMissed?.Invoke();
        }


    }

    class Group
    {
        private string specialization;
        private int course;
        private int count;

        public event Action GroupPartyPlanned;
        public event Action SessionSurvived;

        public int Count
        {
            get => Students.Count;
            private set => count = value;
        }

        public string Specialization
        {
            get => specialization;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidGroupDataException("specialization");
                specialization = value;
            }
        }

        public int Course
        {
            get => course;
            set
            {
                if (value < 1 || value > 6)
                    throw new InvalidGroupDataException("course");
                course = value;
            }
        }

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

        public Student1 this[int index]
        {
            get
            {
                if (index < 0 || index >= Students.Count)
                    throw new IndexOutOfRangeException();
                return Students[index];
            }
            set
            {
                if (index < 0 || index >= Students.Count)
                    throw new IndexOutOfRangeException();
                Students[index] = value;
            }
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
            Students.RemoveAll(s => s.AverageGradeCalc() < minAverage);
        }

        public void ExpelWorst()
        {
            if (Students.Count == 0) return;
            var worst = Students.OrderBy(s => s.AverageGradeCalc()).First();
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

        public delegate bool StudentFilter(Student1 student);
        public List<Student1> FilterStudents(StudentFilter filter)
        {
            var result = new List<Student1>();
            foreach (var s in Students)
                if (filter(s)) result.Add(s);
            return result;
        }

        public void CheckSession()
        {
            if (Students.Count == 0) return;
            bool allExcellent = Students.All(s => s.AverageGradeCalc() >= 10);
            if (allExcellent)
            {
                GroupPartyPlanned?.Invoke();
                SessionSurvived?.Invoke();
            }
        }
    }

    class Program
    {
        static void Main()
        {
            try
            {
                var s1 = new Student1("Alex", "middle", "lastname", new DateTime(2011, 12, 1), "place", "1234567890") { Name = "Alex", Lastname = "lastname22", Age = 20, AverageGrade = 90 };
                var s2 = new Student1("name2", "middle", "lastname", new DateTime(2011, 5, 10), "place", "5551234") { Name = "something", Lastname = "lastname33", Age = 25, AverageGrade = 80 };
                var s3 = new Student1("name3", "middle", "lastname", new DateTime(2011, 8, 20), "place", "5559876") { Name = "something1", Lastname = "lastname44", Age = 40, AverageGrade = 70 };

                var groupA = new Group(new List<Student1> { s1, s2, s3 }, "A1", "object", 1)
                {
                    Specialization = "Programming",
                    Course = 2
                };

                Console.WriteLine("specialization: " + groupA.Specialization);
                Console.WriteLine("course: " + groupA.Course);
                Console.WriteLine("count: " + groupA.Count);
                Console.WriteLine("student1: " + groupA[1].Name);

                s1.AddGrade(s1.Tests, 8);
                s1.AddGrade(s1.Coursework, 10);
                s1.AddGrade(s1.Exams, 8);
                s2.AddGrade(s2.Tests, 7);
                s2.AddGrade(s2.Coursework, 8);
                s2.AddGrade(s2.Exams, 9);
                s3.AddGrade(s3.Tests, 5);
                s3.AddGrade(s3.Coursework, 10);
                s3.AddGrade(s3.Exams, 12);

                groupA.ShowAllStudents();

                var filteredStudents =
                    from st in groupA.Students
                    where st.FirstName.StartsWith("A")
                    where st.AverageGradeCalc() > 7
                    where (st.Tests.Count + st.Coursework.Count + st.Exams.Count) > 5
                    select st;

                foreach (var st in filteredStudents)
                {
                    st.ShowInfo();
                }

                var excellent = groupA.FilterStudents(s => s.AverageGradeCalc() >= 10);
                Console.WriteLine("good:");
                excellent.ForEach(s => s.ShowInfo());

                var nameB = groupA.FilterStudents(s => s.FirstName.StartsWith("Б"));
                Console.WriteLine("nameB");
                nameB.ForEach(s => s.ShowInfo());

                s1.LectureMissed += () => Console.WriteLine("miss");
                s1.AutomatReceived += () => Console.WriteLine("auto passed");
                s1.ScholarshipAwarded += () => Console.WriteLine("awarded scholarship");
                groupA.GroupPartyPlanned += () => Console.WriteLine("group party for everyone passing");
                groupA.SessionSurvived += () => Console.WriteLine("passed");

                s1.CheckTime();
                groupA.CheckSession();
            }
            catch (Exception ex)
            {
                Console.WriteLine("eror: " + ex.Message);
            }
        }
    }
}
