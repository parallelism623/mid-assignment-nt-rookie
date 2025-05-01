import React from "react";
import { Drawer, Form, Button, Space } from "antd";

export function FilterDrawer({
  title,
  open,
  onClose,
  form,
  onSubmit,
  children,
}) {
  return (
    <Drawer
      title={title}
      width={360}
      onClose={onClose}
      open={open}
      bodyStyle={{ paddingBottom: 80 }}
      footer={
        <Space style={{ float: "right" }}>
          <Button onClick={() => form.resetFields()}>Clear</Button>
          <Button type="primary" onClick={() => form.submit()}>
            Apply
          </Button>
        </Space>
      }
    >
      <Form form={form} layout="vertical" onFinish={onSubmit}>
        {children}
      </Form>
    </Drawer>
  );
}
