using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeDBApp
{
    public partial class EmployeeInformationUI : Form
    {
        public EmployeeInformationUI()
        {
            InitializeComponent();
        }
        Employee employee=new Employee();
       public bool emailCheck=false;
        public bool isUpdateMode = false;
        public bool isDeleteMode = false;
        private int employeeID ;
        //public string dialog;
        private void saveButton_Click(object sender, EventArgs e)
        {
            employee.name = nameTextBox.Text;
            employee.address = addresssTextBox.Text;
            employee.email = emailTextBox.Text;
            employee.salary = Convert.ToDouble(salaryTextBox.Text);
            emailCheck = IsEmailExits(employee.email);
            if (isUpdateMode)
            {
                string connectionString =
                                        @"SERVER=PC-301-03\SQLEXPRESS;Database=Employee_Information;Integrated Security=true";
                SqlConnection connection = new SqlConnection(connectionString);
                string query = "UPDATE employees SET name='" + employee.name + "',address='" + employee.address + "',email='" +
                               employee.email + "',salary='" + employee.salary + "'WHERE id='"+employeeID+"'";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                int rowAffected = command.ExecuteNonQuery();
                connection.Close();
                if (rowAffected > 0)
                {
                    MessageBox.Show("Data Updated!");
                    ClearTextBoxes(this.Controls);
                    isUpdateMode = false;
                    saveButton.Text = "Save";
                    isDeleteMode = false;
                    showButon.Text = "Show";
                    employeeID = 0;
                   
                    ShowAllEmployee();
                }
                else
                {
                    MessageBox.Show("Data not Updated!");
                }

            }
            else
            {
                if (emailCheck)
                {
                    MessageBox.Show("Email is exists!");

                }
                else
                {
                    string connectionString =
                        @"SERVER=PC-301-03\SQLEXPRESS;Database=Employee_Information;Integrated Security=true";
                    SqlConnection connection = new SqlConnection(connectionString);
                    string query = "INSERT INTO employees VALUES('" + employee.name + "','" + employee.address + "','" +
                                   employee.email + "','" + employee.salary + "')";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    int rowAffected = command.ExecuteNonQuery();
                    connection.Close();
                    if (rowAffected > 0)
                    {
                        MessageBox.Show("Data saved!");
                        ClearTextBoxes(this.Controls);
                        ShowAllEmployee();
                    }
                    else
                    {
                        MessageBox.Show("Data not saved!");
                    }
                }

            }


        }

        public bool IsEmailExits(string email)
        {
            emailCheck = false;
            string connectionString = @"SERVER=PC-301-03\SQLEXPRESS;Database=Employee_Information;Integrated Security=true";
            SqlConnection connection = new SqlConnection(connectionString);
            string query = "SELECT * FROM employees WHERE email='"+email+"'";
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
               emailCheck=true;
               
            }
            reader.Close();
            connection.Close();
            return emailCheck;

        }

       

        private void showButon_Click(object sender, EventArgs e)
        {
            
            if (isDeleteMode)
            {
                
                DialogResult dialog = MessageBox.Show("Are you sure to delete!","Confirmation",MessageBoxButtons.YesNo);
                if (dialog == DialogResult.Yes)
                {
                    string connectionString =
                        @"SERVER=PC-301-03\SQLEXPRESS;Database=Employee_Information;Integrated Security=true";
                    SqlConnection connection = new SqlConnection(connectionString);
                    string query = "DELETE FROM employees WHERE id='" + employeeID + "'";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    int rowAffected = command.ExecuteNonQuery();
                    connection.Close();
                    if (rowAffected > 0)
                    {
                        
                        MessageBox.Show("Data has been deleted!");

                        ClearTextBoxes(this.Controls);
                        isDeleteMode = false;
                        showButon.Text = "Show";
                        isUpdateMode = false;
                        saveButton.Text = "Save";
                        ShowAllEmployee();
                    }
                    else
                    {
                        MessageBox.Show("Data has not been deleted!");
                    }
                }

            }

            else
            {
                
                ShowAllEmployee();
            }
           
        }

        public void LoadEmployeeListView(List<Employee> employees)
        {
            employeeListView.Items.Clear();
            foreach (var employee in employees)
            {
                ListViewItem item=new ListViewItem(employee.id.ToString());
                item.SubItems.Add(employee.name);
                item.SubItems.Add(employee.address);
                item.SubItems.Add(employee.email);
                item.SubItems.Add(employee.salary.ToString());
                employeeListView.Items.Add(item);
            }
        }

        public void ShowAllEmployee()
        {
            string connectionString = @"SERVER=PC-301-03\SQLEXPRESS;Database=Employee_Information;Integrated Security=true";
            SqlConnection connection = new SqlConnection(connectionString);
            string query = "SELECT * FROM employees";
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Employee> employeeList = new List<Employee>();
            while (reader.Read())
            {
                Employee employee = new Employee();
                employee.id = int.Parse(reader["id"].ToString());
                employee.name = reader["name"].ToString();
                employee.address = reader["address"].ToString();
                employee.email = reader["email"].ToString();
                employee.salary = double.Parse(reader["salary"].ToString());
                employeeList.Add(employee);

            }
            reader.Close();
            connection.Close();
            LoadEmployeeListView(employeeList);
        }

        private void employeeListView_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem item = employeeListView.SelectedItems[0];
            int id = int.Parse(item.Text.ToString());
            Employee employee = GetEmployeeId(id);
            if (employee!=null)
            {
                isUpdateMode = true;
                saveButton.Text = "Update";
                showButon.Text = "Delete";
                isDeleteMode = true;
                employeeID = employee.id;
                nameTextBox.Text = employee.name;
                addresssTextBox.Text = employee.address;
                emailTextBox.Text = employee.email;
                salaryTextBox.Text =employee.salary.ToString();
            }
        }

        public Employee GetEmployeeId(int id)
        {
            string connectionString = @"SERVER=PC-301-03\SQLEXPRESS;Database=Employee_Information;Integrated Security=true";
            SqlConnection connection = new SqlConnection(connectionString);
            string query = "SELECT * FROM employees WHERE id='"+id+"'";
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Employee> employeeList = new List<Employee>();
            while (reader.Read())
            {
                Employee employee = new Employee();
                employee.id = int.Parse(reader["id"].ToString());
                employee.name = reader["name"].ToString();
                employee.address = reader["address"].ToString();
                employee.email = reader["email"].ToString();
                employee.salary = double.Parse(reader["salary"].ToString());
                employeeList.Add(employee);

            }
            reader.Close();
            connection.Close();
            return employeeList.FirstOrDefault();

        }
        private void ClearTextBoxes(Control.ControlCollection cc)
        {
            foreach (Control ctrl in cc)
            {
                TextBox tb = ctrl as TextBox;
                if (tb != null)
                    tb.Text = "";
                else
                    ClearTextBoxes(ctrl.Controls);
            }
        }
    }
}
