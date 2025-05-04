import React, { useEffect, useState } from "react";
import { Table, Typography, Card } from "antd";
import { reportServices } from "../../services/reportServices";

const { Title } = Typography;

const ReportTable = ({ title, columns, data, loading }) => {
  const [pagination, setPagination] = useState({
    current: 1,
    pageSize: 10,
  });

  const handlePageChange = (p, ps) => {
    setPagination({ current: p, pageSize: ps });
  };

  return (
    <Card className="shadow-lg rounded-2xl mt-6">
      <Title level={4} className="mb-4">
        {title}
      </Title>

      <Table
        columns={columns}
        loading={loading}
        dataSource={data}
        rowKey={(record) => record.id}
        showSizeChanger
        pagination={{
          current: pagination.current,
          total: data?.length ?? 0,
          pageSize: pagination.pageSize,
          showSizeChanger: true,
          pageSizeOptions: [5, 10, 20, 50],
          showTotal: (total, [start, end]) =>
            `From ${start} to ${end} items of ${total}`,
          onChange: (p, ps) => {
            handlePageChange(p, ps);
          },
        }}
      />
    </Card>
  );
};

export default ReportTable;
