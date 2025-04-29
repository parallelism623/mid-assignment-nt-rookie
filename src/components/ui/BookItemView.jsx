import React from "react";
import { Card, Typography, Button, Space, Tag } from "antd";
import { FiBookmark, FiShare2 } from "react-icons/fi";

const { Title, Text } = Typography;

const BookItemView = () => (
  <Card
    className="shadow-lg rounded-2xl mb-6"
    headStyle={{ borderBottom: "none", padding: "1.5rem" }}
    bodyStyle={{ padding: 0 }}
    title={
      <div className="flex flex-col gap-1">
        <Title level={3} className="m-0">
          Executive Teams Are Losing Stakeholders’ Confidence. Here’s How to Get
          It Back.
        </Title>
        <div className="flex items-center space-x-2">
          <Tag color="blue" className="uppercase text-sm font-medium">
            Managing Uncertainty
          </Tag>
          <Text type="secondary" className="text-sm">
            Digital Article by <a className="text-blue-600">Ron Carucci</a>
          </Text>
        </div>
      </div>
    }
    extra={
      <Space size="middle">
        <Button type="text" icon={<FiBookmark />}>
          Save
        </Button>
        <Button type="text" icon={<FiShare2 />}>
          Share
        </Button>
        <Text type="secondary">APRIL 28, 2025</Text>
      </Space>
    }
  ></Card>
);

export default BookItemView;
