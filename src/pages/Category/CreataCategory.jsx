import { Card, Form, Modal, Input } from "antd";
import { validationData } from "../../constants/validationData";
import React from "react";
import { categoryServices } from "../../services/categoryServices";
const CreateCategory = ({ onSubmit, onCancel }) => {
  const [form] = Form.useForm();

  const handleOnSubmitForm = () => {
    form.validateFields().then((values) => {
      categoryServices
        .create(values)
        .then(() => {
          onSubmit();
        })
        .catch(() => {
          onCancel();
        });
    });
  };
  const handleOnCancelForm = () => {
    onCancel();
  };
  return (
    <Modal
      title="Create Category"
      visible={true}
      onCancel={handleOnCancelForm}
      onOk={handleOnSubmitForm}
      okText="Save"
      cancelText="Cancel"
      maskClosable={false}
      destroyOnClose
    >
      <Form form={form} layout="vertical">
        <Form.Item
          name="name"
          label="Name"
          rules={[
            { required: true, message: "Category name should not empty" },
            {
              max: validationData.categoryNameMaxLength,
              message: `Category name must be less than or equal ${validationData.categoryNameMaxLength}`,
            },
          ]}
        >
          <Input
            maxLength={validationData.categoryNameMaxLength}
            placeholder="Enter name"
          />
        </Form.Item>

        <Form.Item
          name="description"
          label="Description"
          rules={[
            {
              max: validationData.categoryDescriptionMaxLength,
              message: `Category description should less than or equal ${validationData.categoryDescriptionMaxLength}`,
            },
          ]}
        >
          <Input.TextArea
            maxLength={validationData.categoryDescriptionMaxLength}
            rows={4}
            placeholder="Enter description"
          />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default CreateCategory;
