import React, { useEffect, useState } from "react";
import { reportServices } from "../../services/reportServices";
import ReportTable from "./ReportTable";

const UserEngagementReport = ({ reportQueryParameters }) => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    let isSetData = true;
    setLoading(true);

    reportServices
      .getUserEngagementReport(reportQueryParameters)
      .then((res) => {
        if (isSetData) {
          setData(res.items);
          setLoading(false);
        }
      })
      .catch(() => {
        setLoading(false);
      });

    return () => {
      isSetData = false;
    };
  }, [
    reportQueryParameters.toDate,
    reportQueryParameters.fromDate,
    reportQueryParameters.top,
  ]);

  const columns = [
    {
      title: "Email",
      dataIndex: "email",
      key: "email",
    },
    {
      title: "Username",
      dataIndex: "username",
      key: "username",
    },
    {
      title: "Total Requests",
      dataIndex: "totalBookBorrowingRequest",
      key: "totalBookBorrowingRequest",
    },
    {
      title: "Approved",
      dataIndex: "approvedBookBorrowingRequest",
      key: "approvedBookBorrowingRequest",
    },
    {
      title: "Rejected",
      dataIndex: "rejectedBookBorrowingRequest",
      key: "rejectedBookBorrowingRequest",
    },
    {
      title: "Total Borrowing (Actual)",
      dataIndex: "totalBookBorrowing",
      key: "totalBookBorrowing",
    },
    {
      title: "Active Days",
      dataIndex: "activeDay",
      key: "activeDay",
    },
    {
      title: "Last Borrow Date",
      dataIndex: "lastBorrowedBook",
      key: "lastBorrowedBook",
    },
    {
      title: "Reviews",
      dataIndex: "numberOfBookReview",
      key: "numberOfBookReview",
    },
  ];

  return (
    <ReportTable
      title={`👤 User Engagement Report from ${reportQueryParameters.fromDate} to ${reportQueryParameters.toDate} (Order by total borrowing request)`}
      columns={columns}
      loading={loading}
      data={data}
    />
  );
};

export default UserEngagementReport;
