import React from "react";
import {
  Modal,
  Card,
  Form,
  Input,
  DatePicker,
  Rate,
  Button,
  Row,
  Col,
} from "antd";
import {
  FiBookOpen,
  FiFileText,
  FiCalendar,
  FiStar,
  FiAlignLeft,
} from "react-icons/fi";
import dayjs from "dayjs";
import { validationData } from "../../constants/validationData";
import { bookReviewServices } from "../../services/bookReviewServices";
const BookReviewModal = ({ visible, onCancel, book, reviewerId, onSubmit }) => {
  const [form] = Form.useForm();

  const handleFinish = (values) => {
    const payload = {
      BookId: book.id,
      ReviewerId: reviewerId,
      DateReview: values.dateReview.format("YYYY-MM-DD"),
      Rating: values.rating,
      Title: values.reviewTitle,
      Content: values.content,
    };
    handleSubmitBookReview(payload);
    form.resetFields();
  };
  const handleSubmitBookReview = (bookReview) => {
    bookReviewServices.create(bookReview).then(() => {
      onSubmit();
      onCancel();
    });
  };
  const handleCancel = () => {
    form.resetFields();
    onCancel();
  };

  return (
    <Modal
      centered
      visible={visible}
      onCancel={handleCancel}
      footer={null}
      destroyOnClose
      width={600}
      className="rounded-2xl"
      title={
        <div className="flex items-center space-x-2">
          <FiBookOpen size={20} /> <span className="text-lg">Review Book</span>
        </div>
      }
    >
      <Card
        bordered={false}
        className="mb-6"
        bodyStyle={{ textAlign: "center", padding: "1.5em" }}
      >
        <Card.Meta
          title={<span className="text-xl font-semibold">{book?.title}</span>}
          description={<span className="text-gray-500">by {book?.author}</span>}
        />
      </Card>

      <Form
        form={form}
        layout="vertical"
        initialValues={{ dateReview: dayjs() }}
        onFinish={handleFinish}
      >
        <Row gutter={16}>
          <Col span={12}>
            <Form.Item
              name="reviewTitle"
              label={
                <span className="flex items-center">
                  <FiFileText className="mr-1" /> Review Title
                </span>
              }
              rules={[
                {
                  required: true,
                  message: "Please enter a title for your review",
                },
                {
                  max: validationData.bookReviewTitleMaxLength,
                  message: `Review title must be less than or equal than ${validationData.bookReviewTitleMaxLength}`,
                },
              ]}
            >
              <Input placeholder="Enter review title" prefix={<FiFileText />} />
            </Form.Item>
          </Col>

          <Col span={12}>
            <Form.Item
              name="dateReview"
              label={
                <span className="flex items-center">
                  <FiCalendar className="mr-1" /> Review Date
                </span>
              }
            >
              <DatePicker
                disabled
                style={{ width: "100%" }}
                suffixIcon={<FiCalendar />}
              />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item
          name="rating"
          label={
            <span className="flex items-center">
              <FiStar className="mr-1 text-yellow-400" /> Rating
            </span>
          }
          rules={[{ required: true, message: "Please rate this book" }]}
        >
          <Rate allowClear={false} />
        </Form.Item>

        <Form.Item
          name="content"
          label={
            <span className="flex items-center">
              <FiAlignLeft className="mr-1" /> Content
            </span>
          }
          rules={[
            { required: true, message: "Please write your review" },
            {
              max: validationData.bookReviewContentMaxLength,
              message: `Review content must be less than or equal than ${validationData.bookReviewContentMaxLength}`,
            },
          ]}
        >
          <Input.TextArea rows={4} placeholder="Write your review..." />
        </Form.Item>
        <Form.Item>
          <Button type="primary" htmlType="submit" block className="rounded-lg">
            Submit Review
          </Button>
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default BookReviewModal;
