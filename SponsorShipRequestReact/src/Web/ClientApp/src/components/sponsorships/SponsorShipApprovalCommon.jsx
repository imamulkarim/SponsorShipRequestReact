import React, { useState, useEffect, useRef } from 'react';

import { ManagerApprovalsClient, SponsorshipTypesClient, SponsorshipRequestsClient, FinanceApprovalsClient } from '../../web-api-client.ts';
//import { SponsorshipRequestStatus } from './SponsorshipRequestStatus.jsx';
import { useAuth, SystemRoles } from '../api-authorization/AuthContext.jsx';
import { formatDate } from '../../utils/dateUtils.jsx';

const managerClient = new ManagerApprovalsClient();
const financeClient = new FinanceApprovalsClient();
//const sponsorTypeClient = new SponsorshipTypesClient();
const sponsorClient = new SponsorshipRequestsClient();


export function SponsorShipApprovalCommon({ approvalCategory }) {

    const { userRoles, userEmail } = useAuth();
    const [sponsorShipApprovalRequests, setSponsorShipApprovalRequests] = useState([]);
    const [newListError, setNewListError] = useState('');
    const [requestData, setRequestData] = useState({});
    const [approvalRemarks, setApprovalRemarks] = useState('');

    const sponsReqDetailsDialogRef = useRef(null);


    const loadRequests = async () => {
        try {

            if (approvalCategory === SystemRoles.Manager) {

                const result = await managerClient.getPendingApprovals();
                console.log(result);
                setSponsorShipApprovalRequests(result.pendingApprovals);
            }
            else if (approvalCategory === SystemRoles.FinanceAdmin) {

                const result = await financeClient.getPendingApprovalsFinance();
                //console.log(result.pendingApprovals);
                setSponsorShipApprovalRequests(result.pendingApprovals);
                
            }
        } catch (err) {
            console.error(err);
        }
    };

    useEffect(() => {

        console.log(approvalCategory);

        loadRequests();
    }, [approvalCategory]);


    const viewSponsorShipDetalils = async (item) => {
        sponsReqDetailsDialogRef.current.showModal();
        setNewListError('');
        setApprovalRemarks('');

        sponsorClient.getMyRequestsDetail(item.id).then(result => {
            //console.log(result);
            setRequestData({ ...result, eventDate: formatDate(result.eventDate) });

        }).catch((error) => {
            console.error(error);
        });
    }

    const handleRejectClick = async (item) => {

        //console.log(item);
        if (!approvalRemarks) {
            setNewListError('Remarks is required!!');
            return;
        }

        if (approvalCategory === SystemRoles.Manager) {
            managerClient.rejectRequest(item.id, { id: item.id, remarks: approvalRemarks, approverRole: SystemRoles.Manager }).then(result => {
                sponsReqDetailsDialogRef.current.close();
                loadRequests();

            }).catch((error) => {
                console.error(error);
            });
        }
        else if(approvalCategory === SystemRoles.FinanceAdmin) {
            financeClient.rejectRequestFinance(item.id, { id: item.id, remarks: approvalRemarks, approverRole: SystemRoles.Manager }).then(result => {
                sponsReqDetailsDialogRef.current.close();
                loadRequests();

            }).catch((error) => {
                console.error(error);
            });
        }
    }

    const handleApproveClick = async (item) => {

        //console.log(item);
        if (!approvalRemarks) {
            setNewListError('Remarks is required!!');
            return;
        }

        if (approvalCategory === SystemRoles.Manager) {
            managerClient.approveRequest(item.id, { id: item.id, remarks: approvalRemarks, approverRole: SystemRoles.Manager }).then(result => {
                sponsReqDetailsDialogRef.current.close();
                loadRequests();

            }).catch((error) => {
                console.error(error);
            });
        }
        else if(approvalCategory === SystemRoles.FinanceAdmin) {
            financeClient.approveRequestFinance(item.id, { id: item.id, remarks: approvalRemarks, approverRole: SystemRoles.Manager }).then(result => {
                sponsReqDetailsDialogRef.current.close();
                loadRequests();

            }).catch((error) => {
                console.error(error);
            });
        }
    }


    return (
        <>
            <hgroup>
                <h1>Sponsorship {approvalCategory} Dashboard</h1>
                <p>Approve sponsorship</p>
            </hgroup>

            <div className="todo-layout">

                <div className="todo-panel-header">
                    
                </div>

                {sponsorShipApprovalRequests && (
                    <table style={{ width: '100%' }} >
                        <thead>
                            <tr>
                                <th>Title</th>
                                <th>Event</th>
                                <th>Amount</th>

                                <th>Event Date</th>
                                <th>Submitted</th>
                                <th>Action</th>
                            </tr>
                        </thead>
                        <tbody>
                            {sponsorShipApprovalRequests.map((item, i) =>
                                <tr key={item.id}>
                                    <td>{item.title}</td>
                                    <td>{item.eventName}</td>
                                    <td>{item.requestedAmount}</td>
                                    {/* <td>{item.status}</td> */}
                                    <td>{item.eventDate.toISOString().split('T')[0]}</td>
                                    <td>{item.submittedAt.toISOString().split('T')[0]}</td>
                                    <td>
                                        <button onClick={() => viewSponsorShipDetalils(item)}>View</button>
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                )}

            </div>



            {/* Sponsor ship Details dialog */}
            <dialog ref={sponsReqDetailsDialogRef}>
                <article>
                    <header>
                        <h3>Sponsorship Request details</h3>
                        <button rel="prev" aria-label="Close" onClick={() => { sponsReqDetailsDialogRef.current.close(); }}></button>
                    </header>
                    <div className="flex-row">
                        <label htmlFor="requestor" className="header-cell">Requestor</label>
                        <span id="requestor" className="desc-cell" > {requestData.requestorName} </span>
                    </div>

                    <div className="flex-row">
                        <label htmlFor="department" className="header-cell">Department</label>
                        <span id="department" className="desc-cell"> {requestData.department} </span>
                    </div>

                    <div className="flex-row">
                        <label htmlFor="sponsorshipType" className="header-cell">Type</label>
                        <span id="sponsorshipType" className="desc-cell" > {requestData.sponsorshipType} </span>
                    </div>

                    <div className="flex-row">
                        <label htmlFor="eventName" className="header-cell">Event Name</label>
                        <span id="eventName" className="desc-cell" > {requestData.eventName} </span>
                    </div>

                    <div className="flex-row">
                        <label htmlFor="eventDate" className="header-cell">Event Date</label>
                        <span id="eventDate" className="desc-cell"> {requestData.eventDate} </span>
                    </div>

                    <div className="flex-row">
                        <label htmlFor="requestedAmount" className="header-cell">Requested Amount</label>
                        <span id="requestedAmount" className="desc-cell"> {requestData.requestedAmount} </span>
                    </div>

                    <div className="flex-row">
                        <label htmlFor="purpose" className="header-cell">Purpose</label>
                        <span id="purpose" className="desc-cell"> {requestData.purpose} </span>
                    </div>

                    <div className="flex-row">
                        <label htmlFor="businessBenefit" className="header-cell">Expected Business Benefit</label>
                        <span id="businessBenefit" className="desc-cell"> {requestData.businessBenefit} </span>
                    </div>

                    <div className="flex-row">
                        <label htmlFor="managerApprovalRemarks" className="header-cell">Manager Remarks</label>
                        <span id="managerApprovalRemarks" className="desc-cell"> {requestData.managerApprovalRemarks} </span>
                    </div>

                    <div className="flex-row">
                        <label htmlFor="financeApprovalRemarks" className="header-cell">Finance Remarks</label>
                        <span id="financeApprovalRemarks" className="desc-cell"> {requestData.financeApprovalRemarks} </span>
                    </div>

                    <div className="flex-row">
                        <h6>Approval History</h6>
                        <ul className="list-group">
                            {requestData?.approvalHistory?.map((item, i) => (
                                <li key={i}>
                                    <strong>{item?.approverRole}:</strong> {item?.action}
                                    <br />
                                    <small className="text-muted">{formatDate(item?.approvedAt)}</small>
                                    <br />
                                    {item?.comments}
                                </li>
                            ))}
                        </ul>
                    </div>

                    <div className="flex-row">
                        <label htmlFor="newSponReqEventbusinessBenefit">Remarks*</label>
                        <textarea id="newSponReqEventbusinessBenefit" value={approvalRemarks} onChange={(e) => { setApprovalRemarks(e.currentTarget.value) }} />
                    </div>

                    {newListError && <small>{newListError}</small>}

                    <footer>
                        <button className="danger" style={{ marginInlineEnd: 'auto' }} onClick={() => handleRejectClick(requestData)}>Reject</button>
                        <button onClick={() => handleApproveClick(requestData)}>Approve</button>
                        <button className="secondary" onClick={() => { sponsReqDetailsDialogRef.current.close(); setRequestData({}); }}>Close</button>
                    </footer>
                </article>
            </dialog> 



        </>);

}