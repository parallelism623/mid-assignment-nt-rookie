import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { Form, Input, InputNumber, Card, Button, Select, Row, Col } from "antd";
import { FiBook, FiUser, FiAlignLeft, FiTag, FiHash } from "react-icons/fi";
import { bookServices } from "../../services/bookServices";
import { categoryServices } from "../../services/categoryServices";
import { validationData } from "../../constants/validationData";

const { Option } = Select;

const CreateBook = () => {
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const [categories, setCategories] = useState([]);
  const [loadingCat, setLoadingCat] = useState(false);

  useEffect(() => {
    setLoadingCat(true);
    categoryServices
      .getsName()
      .then((res) => setCategories(res.items))
      .finally(() => setLoadingCat(false));
  }, []);

  const onFinish = (values) => {
    bookServices.create(values).then(() => navigate(-1));
  };

  return (
    <div className="h-fit flex justify-center  p-4">
      <Card
        title={<span className="text-2xl font-bold">Create Book</span>}
        className="w-150 min-h-80 rounded-2xl shadow-lg bg-transparent"
        headStyle={{
          backgroundColor: "transparent",
          borderBottom: "none",
          paddingBottom: 0,
        }}
        bodyStyle={{ backgroundColor: "transparent", padding: "24px" }}
      >
        <Form form={form} layout="vertical" onFinish={onFinish}>
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
                name="categoryId"
                label="* Category"
                rules={[{ required: true, message: "Category is required" }]}
              >
                <Select
                  placeholder="Select category"
                  loading={loadingCat}
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
                  min={0}
                  placeholder="Quantity"
                  className="w-full rounded-lg"
                  formatter={(value) => `${value}`}
                  parser={(value) => value?.replace(/\D/g, "") || ""}
                />
              </Form.Item>
            </Col>
          </Row>

          <Row justify="end" gutter={8} className="mt-4">
            <Col>
              <Button onClick={() => navigate(-1)} className="rounded-lg">
                Back
              </Button>
            </Col>
            <Col>
              <Button type="primary" htmlType="submit" className="rounded-lg">
                Submit
              </Button>
            </Col>
          </Row>
        </Form>
      </Card>
    </div>
  );
};

export default CreateBook;
