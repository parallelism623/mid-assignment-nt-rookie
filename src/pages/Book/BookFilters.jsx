import React, { useState } from "react";
import { PlusOutlined } from "@ant-design/icons";
import { Drawer, Form, Checkbox, Rate, Button, Space } from "antd";
const BookFilter = ({ open, onClose, onSubmit }) => {
  const [form] = Form.useForm();
  return (
    <>
      <Drawer
        title="Filter Books"
        width={"25rem"}
        onClose={onClose}
        open={open}
        styles={{
          body: {
            paddingBottom: 80,
          },
        }}
        extra={<Space></Space>}
      >
        <Form form={form} layout="vertical">
          <Form.Item
            name="available"
            valuePropName="checked"
            style={{ marginBottom: 24 }}
          >
            <Checkbox>Available</Checkbox>
          </Form.Item>

          <Form.Item
            name="rating"
            label="Minimum Rating"
            tooltip="Chỉ hiện sách có số sao ≥ giá trị chọn"
          >
            <Rate allowClear />
          </Form.Item>

          <Form.Item style={{ marginTop: 32 }}>
            <Space>
              <Button>Clear</Button>
              <Button type="primary" htmlType="submit">
                Apply
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </Drawer>
    </>
  );
};
export default BookFilter;
