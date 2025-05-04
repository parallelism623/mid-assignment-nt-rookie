import React, { useEffect, useState } from "react";
import { Form, Input, Button, Card } from "antd";
import { useUserContext } from "../../routes/ProtectedRoute";
import { userServices } from "../../services/userServices";

const UserUpdateProfile = () => {
  const [form] = Form.useForm();
  const [user, setUser] = useState(null);
  const { id, refreshUser } = useUserContext();

  useEffect(() => {
    userServices.getById(id).then((res) => {
      setUser(res);
      form.setFieldsValue({
        firstName: res.firstName,
        lastName: res.lastName,
        phoneNumber: res.phoneNumber,
      });
    });
  }, [id]);
  const onFinish = (values) => {
    userServices.updateProfile(id, values).then(() => {
      refreshUser();
    });
  };
  return (
    <div className="w-full px-4 md:px-10">
      <Card className="rounded-2xl shadow-lg p-6">
        <h2 className="text-xl font-semibold mb-4">Update Your Profile</h2>
        <Form form={form} layout="vertical" onFinish={onFinish}>
          <Form.Item
            name="firstName"
            label="First Name"
            rules={[
              { required: true, message: "Please enter your first name" },
            ]}
          >
            <Input placeholder="Enter your first name" />
          </Form.Item>

          <Form.Item
            name="lastName"
            label="Last Name"
            rules={[{ required: true, message: "Please enter your last name" }]}
          >
            <Input placeholder="Enter your last name" />
          </Form.Item>

          <Form.Item
            name="phoneNumber"
            label="Phone Number"
            rules={[
              { required: true, message: "Please enter your phone number" },
              {
                pattern: /^[0-9]{10,11}$/,
                message: "Invalid phone number format",
              },
            ]}
          >
            <Input placeholder="Enter your phone number" />
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

export default UserUpdateProfile;
