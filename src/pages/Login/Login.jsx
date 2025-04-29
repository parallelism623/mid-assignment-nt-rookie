import React, { useEffect } from "react";
import { Form, Input, Button, Checkbox, Card } from "antd";
import { UserOutlined, LockOutlined } from "@ant-design/icons";
import { useLocalStorage } from "../../components/hooks/useStorage";
import { authenServices } from "../../services/authenServices";
import { useNavigate } from "react-router";
import { routesPath } from "../../constants/routesPath";
import { validationData } from "../../constants/validationData";

const Login = () => {
  const navigate = useNavigate();
  const [accessToken, setAccessToken] = useLocalStorage("access_token", "");
  const [userEmailConfirm, setUserEmailConfirm] = useLocalStorage(
    "user-email-confirm",
    {}
  );
  const [refreshToken, setRefreshToken] = useLocalStorage("refresh_token", "");

  useEffect(() => {
    if (accessToken) {
      navigate("/dashboard");
    }
  }, [accessToken, navigate]);

  const [form] = Form.useForm();
  const onFinish = () => {
    authenServices.login(form.getFieldsValue()).then((res) => {
      if (res.accessToken) {
        setAccessToken(res.accessToken);
        setRefreshToken(res.refreshToken);
        navigate("/");
      } else {
        setUserEmailConfirm(form.getFieldsValue());
        navigate(routesPath.emailConfirm);
      }
    });
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-blue-50 p-4">
      <h1 className="text-4xl font-bold text-center mb-1">Book Management</h1>
      <p className="text-center text-gray-600 mb-6">
        Sign in to continue to your account
      </p>
      <Card
        title={
          <div className="w-full text-center">
            <span className="text-2xl font-bold">Login</span>
          </div>
        }
        className="w-100 p-4 rounded-2xl shadow-md"
        headStyle={{ borderBottom: "none", paddingBottom: 0 }}
        bodyStyle={{ paddingTop: 0 }}
      >
        <Form
          form={form}
          name="login"
          onFinish={onFinish}
          initialValues={{ remember: true }}
          layout="vertical"
          size="large"
        >
          <Form.Item
            name="username"
            rules={[
              { required: true, message: "Please input your username!" },
              {
                pattern: /^[A-Za-z][A-Za-z0-9_]{1,32}$/,
                message: `Username must start with a letter, 1-${validationData.userUserNameMaxLength} chars, only letters, numbers and underscore.`,
              },
            ]}
          >
            <Input
              prefix={<UserOutlined className="text-gray-400" />}
              placeholder="Username"
              className="rounded-lg"
            />
          </Form.Item>

          <Form.Item
            name="password"
            rules={[
              { required: true, message: "Please input your password!" },
              {
                pattern: validationData.userPasswordRegexPattern,
                message: `Password must be ${validationData.userPasswordMinLength}–${validationData.userPasswordMaxLength} chars, include uppercase, lowercase, number and special character.`,
              },
            ]}
          >
            <Input.Password
              prefix={<LockOutlined className="text-gray-400" />}
              placeholder="Password"
              className="rounded-lg"
            />
          </Form.Item>

          <Form.Item>
            <div className="flex justify-between items-center">
              <Form.Item name="remember" valuePropName="checked" noStyle>
                <Checkbox>Remember me</Checkbox>
              </Form.Item>
            </div>
          </Form.Item>

          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              block
              className="rounded-lg"
            >
              Log in
            </Button>
          </Form.Item>

          <div className="text-center text-gray-600">
            Don't have an account?{" "}
            <a href="/register" className="text-blue-500 hover:underline">
              Register now
            </a>
          </div>
        </Form>
      </Card>
    </div>
  );
};

export default Login;
