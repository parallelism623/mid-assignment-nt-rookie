import { Modal, Card, Form, Input, InputNumber, Select, Row, Col } from "antd";
import { useEffect, useState } from "react";
import { roleServices } from "../../services/roleServices";

const { Option } = Select;

const CreateUserModal = ({ visible, onCancel, onSubmit }) => {
  const [form] = Form.useForm();
  const [roles, setRoles] = useState([]);
  useEffect(() => {
    roleServices.get().then((res) => {
      setRoles([...res]);
    });
  }, []);
  const handleOk = () => {
    form
      .validateFields()
      .then((values) => {
        onSubmit(values);
        onCancel();
      })
      .catch((info) => {
        onCancel();
      });
  };

  return (
    <Modal
      title="Edit User"
      visible={visible}
      onCancel={onCancel}
      onOk={handleOk}
      okText="Save"
    >
      <Card>
        <Form form={form} layout="vertical">
          <Row gutter={[16, 16]}>
            <Col md={12} sm={24}>
              <Form.Item
                label="Username"
                name="username"
                rules={[
                  { required: true, message: "Username is required" },
                  {
                    pattern: /^[A-Za-z0-9_]{3,32}$/,
                    message:
                      "Username must be 3-32 characters and only letters, numbers, or underscore",
                  },
                ]}
              >
                <Input />
              </Form.Item>
            </Col>

            <Col md={12} sm={24}>
              <Form.Item
                label="Email"
                name="email"
                rules={[
                  { required: true, message: "Email is required" },
                  {
                    pattern: /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/,
                    message: "Invalid email format",
                  },
                  {
                    max: 100,
                    message: "Email must be ≤ 100 characters",
                  },
                ]}
              >
                <Input />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item
            label="Password"
            name="password"
            rules={[
              { required: true, message: "Password is required" },
              {
                pattern:
                  /^(?=.{8,32}$)(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/,
                message:
                  "Password must be 8-32 characters, include uppercase, lowercase, number and special character",
              },
            ]}
          >
            <Input.Password />
          </Form.Item>
          <Form.Item
            label="Confirm Password"
            name="confirmPassword"
            dependencies={["password"]}
            validateFirst
            rules={[
              { required: true, message: "Please confirm your password" },
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue("password") === value) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error("Passwords do not match"));
                },
              }),
            ]}
          >
            <Input.Password />
          </Form.Item>

          <Form.Item
            label="Role"
            name="roleId"
            rules={[{ required: true, message: "Role is required" }]}
          >
            <Select>
              {roles.map((r) => (
                <Option key={r.id} value={r.id}>
                  {r.name}
                </Option>
              ))}
            </Select>
          </Form.Item>

          <Row gutter={[16, 16]}>
            <Col md={12} sm={24}>
              <Form.Item
                label="First Name"
                name="firstName"
                rules={[
                  { required: true, message: "First name is required" },
                  { max: 100, message: "Max 100 characters allowed" },
                ]}
              >
                <Input />
              </Form.Item>
            </Col>
            <Col md={12} sm={24}>
              <Form.Item
                label="Last Name"
                name="lastName"
                rules={[
                  { required: true, message: "Last name is required" },
                  { max: 100, message: "Max 100 characters allowed" },
                ]}
              >
                <Input />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item
            label="Phone Number"
            name="phoneNumber"
            rules={[
              {
                max: 20,
                message: "Phone number must be ≤ 20 characters",
              },
            ]}
          >
            <Input />
          </Form.Item>
        </Form>
      </Card>
    </Modal>
  );
};

export default CreateUserModal;
