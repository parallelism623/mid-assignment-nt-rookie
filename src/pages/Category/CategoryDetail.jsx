import React, { useEffect, useState } from "react";
import { Modal, Form, Input, Button } from "antd";
import { categoryServices } from "../../services/categoryServices";

const CategoryDetail = ({ categoryId, onCancel }) => {
  const [form] = Form.useForm();
  useEffect(() => {
    categoryServices.getById(categoryId).then((res) => {
      form.setFieldsValue({ ...res });
    });
  }, [categoryId]);

  return (
    <Modal
      title="Category details"
      onCancel={onCancel}
      okText="Save"
      visible={true}
      cancelText="Cancel"
      maskClosable={false}
      destroyOnClose
      footer={[
        <Button key="cancel" onClick={onCancel}>
          Cancel
        </Button>,
      ]}
    >
      <Form form={form} layout="vertical">
        <Form.Item name="name" label="Name" disabled>
          <Input disabled placeholder="Enter name" />
        </Form.Item>

        <Form.Item name="description" label="Description" disabled>
          <Input.TextArea
            disabled
            maxLength={2000}
            rows={4}
            placeholder="Enter description"
          />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default CategoryDetail;
