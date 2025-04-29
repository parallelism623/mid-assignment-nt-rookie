import React from "react";
import { Card, Form, Input, Button, Checkbox, Row, Col } from "antd";
import {
  UserOutlined,
  MailOutlined,
  LockOutlined,
  PhoneOutlined,
} from "@ant-design/icons";
import { authenServices } from "../../services/authenServices";
import { useNavigate } from "react-router";
import { validationData } from "../../constants/validationData";

const Register = () => {
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const onFinish = (values) => {
    authenServices.register(values).then(() => {
      navigate("/email-confirm");
    });
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-blue-50 p-4">
      <h1 className="text-4xl font-bold text-center mb-1">Book Management</h1>
      <p className="text-center text-gray-600 mb-6">Create your new account</p>

      <Card
        title={
          <div className="w-full text-center">
            <span className="text-2xl font-bold">Register</span>
          </div>
        }
        className="w-110 p-4 rounded-2xl shadow-md"
        headStyle={{ borderBottom: "none", paddingBottom: 0 }}
        bodyStyle={{ paddingTop: 0 }}
      >
        <Form
          form={form}
          name="register"
          layout="vertical"
          onFinish={onFinish}
          scrollToFirstError
        >
          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                name="firstName"
                label="First Name"
                rules={[
                  { required: true, message: "Please input your first name!" },
                  { max: 100, message: "Max length is 100 characters." },
                ]}
              >
                <Input
                  prefix={<UserOutlined />}
                  placeholder="First Name"
                  className="rounded-lg"
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="lastName"
                label="Last Name"
                rules={[
                  { required: true, message: "Please input your last name!" },
                  { max: 100, message: "Max length is 100 characters." },
                ]}
              >
                <Input
                  prefix={<UserOutlined />}
                  placeholder="Last Name"
                  className="rounded-lg"
                />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                name="username"
                label="Username"
                rules={[
                  { required: true, message: "Please input your username!" },
                  {
                    pattern: /^[A-Za-z][A-Za-z0-9_]{1,32}$/,
                    message: `Username must start with a letter, 1-${validationData.userUserNameMaxLength} chars, only letters, numbers and underscore.`,
                  },
                ]}
              >
                <Input
                  prefix={<UserOutlined />}
                  placeholder="Username"
                  className="rounded-lg"
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="email"
                label="Email"
                rules={[
                  { type: "email", message: "Not a valid email!" },
                  { required: true, message: "Please input your email!" },
                  {
                    max: validationData.userEmailMaxLength,
                    message: `Email must be less than or equal ${validationData.userEmailMaxLength}`,
                  },
                ]}
              >
                <Input
                  prefix={<MailOutlined />}
                  placeholder="Email"
                  className="rounded-lg"
                />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                name="password"
                label="Password"
                rules={[
                  { required: true, message: "Please input your password!" },
                  {
                    pattern: validationData.userPasswordRegexPattern,
                    message: `Password must be ${validationData.userPasswordMinLength}–${validationData.userPasswordMaxLength} chars, include uppercase, lowercase, number and special character.`,
                  },
                ]}
                hasFeedback
              >
                <Input.Password
                  prefix={<LockOutlined />}
                  placeholder="Password"
                  className="rounded-lg"
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="confirm"
                label="Confirm Password"
                dependencies={["password"]}
                hasFeedback
                rules={[
                  { required: true, message: "Please confirm your password!" },
                  ({ getFieldValue }) => ({
                    validator(_, value) {
                      if (!value || getFieldValue("password") === value) {
                        return Promise.resolve();
                      }
                      return Promise.reject(
                        new Error("Passwords do not match!")
                      );
                    },
                  }),
                ]}
              >
                <Input.Password
                  prefix={<LockOutlined />}
                  placeholder="Confirm Password"
                  className="rounded-lg"
                />
              </Form.Item>
            </Col>
          </Row>
          <Form.Item
            name="phoneNumber"
            label="PhoneNumber"
            rules={[
              {
                pattern: /^\d{1,20}$/,
                message: "Phone number must be numeric and up to 20 digits.",
              },
            ]}
          >
            <Input
              prefix={<PhoneOutlined />}
              placeholder="Phone number"
              className="rounded-lg"
            />
          </Form.Item>
          <Form.Item
            name="agreement"
            valuePropName="checked"
            rules={[
              {
                validator: (_, v) =>
                  v
                    ? Promise.resolve()
                    : Promise.reject("You must accept terms"),
              },
            ]}
          >
            <Checkbox>I agree to the Terms and Conditions</Checkbox>
          </Form.Item>

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              block
              className="rounded-lg"
            >
              Register
            </Button>
          </Form.Item>

          <div className="text-center text-gray-600">
            Already have an account?{" "}
            <a href="/login" className="text-blue-500 hover:underline">
              Log in
            </a>
          </div>
        </Form>
      </Card>
    </div>
  );
};

export default Register;
