import { environment } from "../../constants/environment";
import { useUserContext } from "../../routes/ProtectedRoute";

import {
  Table,
  Card,
  Tooltip,
  Tag,
  Form,
  Row,
  Col,
  DatePicker,
  Checkbox,
} from "antd";
import { FiCheck, FiX, FiEye } from "react-icons/fi";
import { IoFilterSharp } from "react-icons/io5";
import { useState, useEffect } from "react";
import { defaultQueryParameters } from "../../constants/queryParameters";
import { booksBorrowingRequestServices } from "../../services/booksBorrowingRequestServices";
import { userServices } from "../../services/userServices";
import dayjs from "dayjs";
import BookBorrowingDetail from "./BookBorrowingDetail";
import { booksBorrowingRequestStatus } from "../../constants/booksBorrowingRequestStatus";
import { FilterDrawer } from "../../components/ui/drawers/filter-drawer";
const { Column } = Table;
const statusOptions = [
  { label: "Waiting", value: "0" },
  { label: "Approved", value: "1" },
  { label: "Rejected", value: "2" },
];

import { useWatch } from "antd/es/form/Form";
const BookBorrowingRequest = () => {
  const { roleName, id, firstName, lastName } = useUserContext();
  const [queryParameters, setQueryParameters] = useState({
    ...defaultQueryParameters,
    status: "111",
  });
  const [showDetailRequest, setShowDetailRequest] = useState(false);
  const [selectedDetailRequest, setSelectedDetailRequest] = useState(null);
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [openDrawerFilter, setOpenDrawerFilter] = useState(false);
  const [form] = Form.useForm();
  const fetchData = (isSetData = true) => {
    setLoading(true);
    if (roleName === environment.adminRole) {
      booksBorrowingRequestServices
        .gets(queryParameters)
        .then((res) => {
          setData(res.items);
          setQueryParameters({
            ...queryParameters,
            totalCount: res.totalCount,
          });
          setLoading(false);
        })
        .catch(() => setLoading(false));
    } else
      userServices
        .getBookBorrowingRequests(queryParameters, id)
        .then((res) => {
          setData(res.items);
          setQueryParameters({
            ...queryParameters,
            totalCount: res.totalCount,
          });
          setLoading(false);
        })
        .catch(() => setLoading(false));
  };
  useEffect(() => {
    let isSetData = true;
    fetchData(isSetData);
    return () => {
      isSetData = false;
    };
  }, [
    queryParameters.pageIndex,
    queryParameters.pageSize,
    queryParameters.totalCount,
    queryParameters.search,
    queryParameters.status,
    queryParameters.fromRequestedDate,
    queryParameters.toRequestedDate,
  ]);
  const handlePageChange = (pageIndex, pageSize) => {
    setQueryParameters({
      ...queryParameters,
      pageIndex: pageIndex,
      pageSize: pageSize,
    });
  };
  const handleOnUpdateRequestStatus = (id, status) => {
    setSelectedDetailRequest(null);
    setShowDetailRequest(false);
    setLoading(true);
    booksBorrowingRequestServices
      .changeStatus({
        id: id,
        status: status,
      })
      .then(() => {
        fetchData()
          .then(() => {
            setLoading(false);
          })
          .catch(() => {
            setLoading(false);
          });
      })
      .catch(() => setLoading(false));
  };
  const handleOnApproveRequest = (id) => {
    handleOnUpdateRequestStatus(id, booksBorrowingRequestStatus.Approved);
  };
  const handleOnRejectRequest = (id) => {
    handleOnUpdateRequestStatus(id, booksBorrowingRequestStatus.Rejected);
  };
  const handleOnViewDetailRequest = (id) => {
    setSelectedDetailRequest(id);
    setShowDetailRequest(true);
  };
  const handleCancelViewDetailRequest = () => {
    setSelectedDetailRequest(null);
    setShowDetailRequest(false);
  };
  const handleOnCloseFilterDrawer = () => {
    setOpenDrawerFilter(false);
  };

  const handleSubmitFilter = (values) => {
    setOpenDrawerFilter(false);
    const statusString = statusOptions
      .map((option) => (values?.status?.includes(option.value) ? "1" : "0"))
      .join("");
    const fromRequestedDate = values.fromRequestedDate?.format("YYYY-MM-DD");
    const toRequestedDate = values.toRequestedDate?.format("YYYY-MM-DD");
    setQueryParameters({
      ...queryParameters,
      ...values,
      toRequestedDate: toRequestedDate,
      fromRequestedDate: fromRequestedDate,
      status: statusString,
    });
  };

  const fromDate = useWatch("fromRequestedDate", form);
  const toDate = useWatch("toRequestedDate", form);
  return (
    <>
      <FilterDrawer
        title={"Book borrowing request filter"}
        open={openDrawerFilter}
        onClose={handleOnCloseFilterDrawer}
        form={form}
        onSubmit={(values) => {
          handleSubmitFilter({ ...values });
        }}
        initialValues={{ status: statusOptions.map((option) => option.value) }}
      >
        <Form.Item label="Status" name="status">
          <Checkbox.Group>
            {statusOptions.map((option) => (
              <Checkbox key={option.value} value={option.value}>
                <span> {option.label}</span>
              </Checkbox>
            ))}
          </Checkbox.Group>
        </Form.Item>
        <Row gutter={[16, 16]}>
          <Col md={12} sm={24}>
            <Form.Item name="fromRequestedDate" label="From Date Request">
              <DatePicker
                style={{ width: "100%" }}
                format="YYYY-MM-DD"
                disabledDate={(current) => {
                  return toDate
                    ? current && dayjs(current).isAfter(dayjs(toDate), "day")
                    : false;
                }}
              />
            </Form.Item>
          </Col>

          <Col md={12} sm={24}>
            <Form.Item name="toRequestedDate" label="To Date Request">
              <DatePicker
                style={{ width: "100%" }}
                format="YYYY-MM-DD"
                disabledDate={(current) => {
                  return fromDate
                    ? current && dayjs(current).isBefore(dayjs(fromDate), "day")
                    : false;
                }}
              />
            </Form.Item>
          </Col>
        </Row>
      </FilterDrawer>

      {showDetailRequest && (
        <BookBorrowingDetail
          bookBorrowingId={selectedDetailRequest}
          onApprove={handleOnApproveRequest}
          onReject={handleOnRejectRequest}
          onCancel={handleCancelViewDetailRequest}
        />
      )}
      <Card
        title="Books borrowing request"
        className="book-list-card be-vietnam-pro-regular"
        extra={
          <Row className="flex items-center gap-2">
            <Col
              className="flex items-center cursor-pointer"
              onClick={() => setOpenDrawerFilter(true)}
            >
              Filters <IoFilterSharp className="ml-1" />
            </Col>
          </Row>
        }
      >
        <Table
          bordered
          className="overflow-x-scroll"
          loading={loading}
          dataSource={data}
          pagination={{
            current: queryParameters.pageIndex,
            total: queryParameters.totalCount,
            pageSize: queryParameters.pageSize,
            showSizeChanger: true,
            pageSizeOptions: [5, 10, 20, 50],
            showTotal: (total, [start, end]) =>
              `From ${start} to ${end} items of ${total}`,
            onChange: (p, ps) => {
              handlePageChange(p, ps);
            },
          }}
        >
          {roleName == environment.adminRole && (
            <Column
              onHeaderCell={() => ({
                className: "be-vietnam-pro-medium",
              })}
              title="Requester"
              key="requester"
              dataIndex={["requester", "fullName"]}
            />
          )}
          <Column
            onHeaderCell={() => ({
              className: "be-vietnam-pro-medium",
            })}
            title="Approver"
            key="approver"
            dataIndex={["approver", "fullName"]}
          />
          <Column
            onHeaderCell={() => ({
              className: "be-vietnam-pro-medium",
            })}
            title="Date Requested"
            key="dateRequested"
            dataIndex="dateRequested"
          />
          <Column
            onHeaderCell={() => ({
              className: "be-vietnam-pro-medium",
            })}
            title="Date Approved"
            key="dateApproved"
            dataIndex="dateApproved"
          />
          <Column
            onHeaderCell={() => ({
              className: "be-vietnam-pro-medium",
            })}
            title="Number of books"
            key="booksBorrowingNumber"
            dataIndex="booksBorrowingNumber"
            width={170}
            align={"center"}
            render={(booksBorrowingNumber) => (
              <Tag color="blue">{booksBorrowingNumber}</Tag>
            )}
          />
          <Column
            title="Status"
            key="status"
            align={"center"}
            dataIndex="status"
            onHeaderCell={() => ({
              className: "be-vietnam-pro-medium",
            })}
            render={(status) => {
              let color = "default";
              let text = "Unknown";

              switch (status) {
                case 0:
                  color = "warning";
                  text = "Waiting";
                  break;
                case 1:
                  color = "success";
                  text = "Approved";
                  break;
                case 2:
                  color = "error";
                  text = "Rejected";
                  break;
                case 3:
                  color = "gray";
                  text = "Overdue";
                  break;
              }

              return <Tag color={color}>{text}</Tag>;
            }}
          />

          <Column
            key="action"
            title="Action"
            onHeaderCell={() => ({
              className: "be-vietnam-pro-medium",
            })}
            render={(_, record) => (
              <div className="flex gap-2">
                {record.status === 0 && roleName === environment.adminRole && (
                  <Tooltip title="Approve">
                    <button
                      onClick={() => handleOnApproveRequest(record.id)}
                      className="p-2 rounded-lg bg-green-300 text-green-800 hover:bg-green-400 cursor-pointer"
                    >
                      <FiCheck size={20} />
                    </button>
                  </Tooltip>
                )}

                {record.status === 0 && roleName === environment.adminRole && (
                  <Tooltip title="Reject">
                    <button
                      onClick={() => handleOnRejectRequest(record.id)}
                      className="p-2 rounded-lg bg-red-300 text-red-800 hover:bg-red-400 cursor-pointer"
                    >
                      <FiX size={20} />
                    </button>
                  </Tooltip>
                )}
                <Tooltip title="Detail" className="cursor-pointer">
                  <button
                    onClick={() => handleOnViewDetailRequest(record.id)}
                    className="p-2 rounded-lg bg-emerald-300 text-emerald-800 hover:bg-emerald-400 cursor-pointer"
                  >
                    <FiEye size={20} />
                  </button>
                </Tooltip>
              </div>
            )}
          />
        </Table>
      </Card>
    </>
  );
};

export default BookBorrowingRequest;
