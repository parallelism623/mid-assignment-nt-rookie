import React, { useEffect, useState } from "react";
import { Drawer, Form, Checkbox, Rate, Button, Space, Select } from "antd";
import { categoryServices } from "../../services/categoryServices";
const BookFilter = ({ open, onClose, onSubmit }) => {
  const [form] = Form.useForm();
  const [categories, setCategories] = useState([]);

  useEffect(() => {
    categoryServices
      .getsName()
      .then((res) => setCategories(res.items))
      .catch(() => {});
  }, []);

  const handOnSubmitForm = () => {
    form.validateFields().then((values) => {
      const filterParameters = {
        rating: !!values.rating ? values.rating : 0,
        categoryIds: !!values.categoryIds ? values.categoryIds : null,
        available: !!values.available ? values.available : false,
      };
      onSubmit(filterParameters);
    });
  };
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
        <Form form={form} layout="vertical" onFinish={handOnSubmitForm}>
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
            tooltip="Just show book that have rating greater than value"
          >
            <Rate allowClear />
          </Form.Item>
          <Form.Item name="categoryIds" label="Categories">
            <Select
              mode="multiple"
              placeholder="Select categories"
              style={{ width: "100%" }}
            >
              {categories.map((c) => (
                <Option key={c.id} value={c.id}>
                  {c.name}
                </Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item style={{ marginTop: 32 }}>
            <Space>
              <Button onClick={() => form.resetFields()}>Clear</Button>
              <Button
                onClick={handOnSubmitForm}
                type="primary"
                htmlType="submit"
              >
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
