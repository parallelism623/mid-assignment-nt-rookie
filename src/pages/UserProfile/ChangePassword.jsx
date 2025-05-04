import React from "react";
import { Form, Input, Button, Card, message } from "antd";
import { authenServices } from "../../services/authenServices";

const ChangePassword = () => {
  const [form] = Form.useForm();

  const passwordRegex =
    /^(?=.{8,32}$)(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/;

  const onSubmit = (values) => {
    authenServices.changePassword(values);
  };
  const handleFinish = async (values) => {
    try {
      await onSubmit(values);

      form.resetFields();
    } catch (error) {}
  };

  return (
    <div className="w-full px-4 md:px-10 flex justify-center">
      <Card className="rounded-2xl shadow-lg p-6 w-full max-w-xl">
        <h2 className="text-xl font-semibold mb-4">Change Your Password</h2>

        <Form form={form} layout="vertical" onFinish={handleFinish}>
          <Form.Item
            label="Current Password"
            name="oldPassword"
            rules={[
              { required: true, message: "Please enter your current password" },
            ]}
          >
            <Input.Password placeholder="Enter current password" />
          </Form.Item>

          <Form.Item
            label="New Password"
            name="password"
            rules={[
              { required: true, message: "Please enter a new password" },
              {
                pattern: passwordRegex,
                message:
                  "Password must be 8–32 characters and include uppercase, lowercase, number, and special character",
              },
            ]}
            hasFeedback
          >
            <Input.Password placeholder="Enter new password" />
          </Form.Item>

          <Form.Item
            label="Confirm New Password"
            name="confirmPassword"
            dependencies={["password"]}
            hasFeedback
            rules={[
              { required: true, message: "Please confirm your new password" },
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
            <Input.Password placeholder="Re-enter new password" />
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit" className="w-full">
              Save Changes
            </Button>
          </Form.Item>
        </Form>
      </Card>
    </div>
  );
};

export default ChangePassword;
