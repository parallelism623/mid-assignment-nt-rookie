import React, { useEffect, useState } from "react";
import { Modal, Form, Input } from "antd";
import { categoryServices } from "../../services/categoryServices";
import { validationData } from "../../constants/validationData";

const CategoryEdit = ({
  visible,
  confirmLoading = false,
  onCancel,
  onSave,
  categoryId,
}) => {
  const [form] = Form.useForm();
  const [category, setCategory] = useState(null);
  const handleOk = () => {
    form
      .validateFields()
      .then(() => {
        onSave(category);
        onCancel();
      })
      .catch(() => {});
  };
  useEffect(() => {
    categoryServices.getById(categoryId).then((res) => {
      setCategory({ ...res });
      form.setFieldsValue({ ...res });
    });
  }, [categoryId]);
  const onInputChange = (e, all) => {
    setCategory({ ...category, ...all });
  };
  return (
    <Modal
      title="Category edit"
      visible={visible}
      onCancel={onCancel}
      onOk={handleOk}
      okText="Save"
      cancelText="Cancel"
      confirmLoading={confirmLoading}
      maskClosable={false}
      destroyOnClose
    >
      <Form form={form} layout="vertical" onValuesChange={onInputChange}>
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

export default CategoryEdit;
