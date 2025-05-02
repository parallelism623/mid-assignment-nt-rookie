import React, { useEffect } from "react";
import { Drawer, Form, Button, Space } from "antd";

export function FilterDrawer({
  title,
  open,
  onClose,
  form,
  onSubmit,
  children,
  initialValues,
}) {
  const handleOnSubmit = () => {
    form.validateFields().then((values) => {
      onSubmit(values);
    });
  };
  useEffect(() => {
    if (initialValues) form.setFieldsValue(initialValues);
  }, []);
  return (
    <Drawer
      title={title}
      width={"25rem"}
      onClose={onClose}
      open={open}
      bodyStyle={{ paddingBottom: 80 }}
      footer={
        <Space style={{ float: "right" }}>
          <Button onClick={() => form.resetFields()}>Clear</Button>
          <Button type="primary" onClick={() => handleOnSubmit()}>
            Apply
          </Button>
        </Space>
      }
    >
      <Form form={form} layout="vertical">
        {children}
      </Form>
    </Drawer>
  );
}
