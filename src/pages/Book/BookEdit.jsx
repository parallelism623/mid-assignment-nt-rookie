import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router";
import { Form, Input, InputNumber, Card, Button, Row, Col, Select } from "antd";
import { FiBook, FiUser, FiTag, FiHash, FiAlignLeft } from "react-icons/fi";
import { bookServices } from "../../services/bookServices";
import { categoryServices } from "../../services/categoryServices";
import { validationData } from "../../constants/validationData";
const { Option } = Select;

const BookEdit = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const [categories, setCategories] = useState([]);
  const [book, setBook] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    let active = true;
    setLoading(true);
    bookServices
      .getById(id)
      .then((res) => {
        if (active) {
          setBook(res);
          form.setFieldsValue(res);
        }
      })
      .finally(() => active && setLoading(false));
    return () => {
      active = false;
    };
  }, [id, form]);

  useEffect(() => {
    categoryServices
      .getsName()
      .then((res) => setCategories(res.items))
      .catch(() => {});
  }, []);

  const onValuesChange = (_, all) => {
    setBook({ ...book, ...all });
  };

  const handleSubmit = () => {
    form.validateFields().then(() => {
      const payload = { ...book, categoryId: book.category?.id };
      bookServices.update(payload).then(() => navigate(-1));
    });
  };

  return (
    <div className="h-fit flex justify-center  p-4">
      <Card
        title={
          <div className="w-full text-center">
            <span className="text-2xl font-bold">Edit Book</span>
          </div>
        }
        className="w-150 p-4 rounded-2xl shadow-md"
        headStyle={{ borderBottom: "none", paddingBottom: 0 }}
        bodyStyle={{ paddingTop: 0 }}
      >
        <Form form={form} layout="vertical" onValuesChange={onValuesChange}>
          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                name="title"
                label="* Title"
                rules={[
                  { required: true, message: "Title is required" },
                  {
                    max: validationData.bookTitleMaxLength,
                    message: `Title must be less than or equal ${validationData.bookTitleMaxLength}`,
                  },
                ]}
              >
                <Input
                  prefix={<FiBook className="text-gray-400 mr-2" />}
                  placeholder="Title"
                  className="rounded-lg"
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="author"
                label="* Author"
                rules={[
                  { required: true, message: "Author is required" },
                  {
                    max: validationData.bookAuthorMaxLength,
                    message: `Author must be less than or equal ${validationData.bookAuthorMaxLength}`,
                  },
                ]}
              >
                <Input
                  prefix={<FiUser className="text-gray-400 mr-2" />}
                  placeholder="Author"
                  className="rounded-lg"
                />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                name="description"
                label="Description"
                rules={[
                  {
                    max: validationData.bookDescriptionMaxLength,
                    message: `Description must be less than or equal ${validationData.bookDescriptionMaxLength}`,
                  },
                ]}
              >
                <Input
                  prefix={<FiAlignLeft className="text-gray-400 mr-2" />}
                  placeholder="Description"
                  className="rounded-lg"
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name={["category", "id"]}
                label="* Category"
                rules={[{ required: true, message: "Category is required" }]}
              >
                <Select
                  placeholder="Select category"
                  loading={loading}
                  className="rounded-lg"
                  suffixIcon={<FiTag />}
                >
                  {categories.map((c) => (
                    <Option key={c.id} value={c.id}>
                      {c.name}
                    </Option>
                  ))}
                </Select>
              </Form.Item>
            </Col>
          </Row>
          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="quantity" label="Quantity">
                <InputNumber
                  disabled
                  placeholder="Quantity"
                  className="!w-full !rounded-lg "
                />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="available" label="Available">
                <InputNumber
                  disabled
                  placeholder="Available"
                  className="!w-full rounded-lg"
                />
              </Form.Item>
            </Col>
          </Row>

          <Form.Item name="addedQuantity" label="Added Quantity">
            <InputNumber
              min={-(book?.available ?? 0)}
              placeholder="Added Quantity"
              className="!w-2/4 rounded-lg"
            />
          </Form.Item>

          <Row justify="end" gutter={8} className="mt-4">
            <Col>
              <Button onClick={() => navigate(-1)} className="rounded-lg">
                Back
              </Button>
            </Col>
            <Col>
              <Button
                type="primary"
                onClick={handleSubmit}
                className="rounded-lg"
              >
                Submit
              </Button>
            </Col>
          </Row>
        </Form>
      </Card>
    </div>
  );
};

export default BookEdit;
