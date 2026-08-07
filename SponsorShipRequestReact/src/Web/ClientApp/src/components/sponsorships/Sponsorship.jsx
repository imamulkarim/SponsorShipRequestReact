import React, { useState, useRef, useEffect, createContext, useContext } from 'react';
import { Plus, Settings, MoreHorizontal } from 'lucide-react';


import { SponsorshipRequestsClient, SponsorshipTypesClient } from '../../web-api-client.ts';
import { CreateOrUpdateSponsorship } from './CreateOrUpdateSponsorship.jsx';
import { SponsorshipRequestStatus } from './SponsorshipRequestStatus.jsx';

const sponsorClient = new SponsorshipRequestsClient();
const sponsorTypeClient = new SponsorshipTypesClient();

const SponsorshipContext = createContext(null);


export function Sponsorship() {

    const [sponsorShipRequests, setsponsorShipRequests] = useState([]);
    
    const [requestData, setRequestData] = useState({});

    const [isEditMode, setIsEditMode] = useState(false);
    const [editItemInfo, seteditItemInfo] = useState({});
    const [cancelReason, setCancelReason] = useState('');
    const [newListError, setNewListError] = useState('');

    const [canEdit, setCanEdit] = useState(false);
    const [canSubmit, setCanSubmit] = useState(false);
    const [canCancel, setCanCancel] = useState(false);


    const newSponsReqDialogRef = useRef(null);
    const sponsReqDetailsDialogRef = useRef(null);

    const loadRequests = async () => {
        try {
            const result = await sponsorClient.getMyRequests();
            setsponsorShipRequests(result.requests);
        } catch (err) {
            console.error(err);
        }
    };

    useEffect(() => {
        loadRequests();
    }, []);

    //todo
    /*
     private checkIfRequestor(): boolean {
    return !this.userRole || this.userRole === 'Requestor';
  }
    */

    useEffect(() => {

        setCanEdit(requestData && requestData?.status === SponsorshipRequestStatus.Draft);
        setCanSubmit(requestData && requestData?.status === SponsorshipRequestStatus.Draft);

        const isCancel = requestData && ![
            SponsorshipRequestStatus.Approved,
            SponsorshipRequestStatus.Rejected,
            SponsorshipRequestStatus.Cancelled
        ].includes(requestData.status);

        setCanCancel(isCancel);

        console.log(requestData);


    }, [requestData]);

    const showListOptionsDialog = () => {
        // setListOptionsEditor({ id: selectedList.id, title: selectedList.title, colour: selectedList.colour || colours[0].code });
        // listOptionsDialogRef.current.showModal();
    };


    // ── Create requst modal ──────────────────────────────────────────────────────────────────

    const showNewSponReqDialog = () => {
        //reset(); // reset to defaults when opening dialog
        newSponsReqDialogRef.current.showModal();
        setIsEditMode(false);
        //setTimeout(() => document.getElementById('newSponReqTitle')?.focus(), 50);
    };

    const formatDate = (date) => {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, "0");
        const day = String(date.getDate()).padStart(2, "0");

        return `${year}-${month}-${day}`;
    };
    

    const viewSponsorShipDetalils = async (item) => {
        sponsReqDetailsDialogRef.current.showModal();
        setNewListError('');
        setCancelReason('');

        sponsorClient.getMyRequestsDetail(item.id).then(result => {
            console.log(result);
            setRequestData(prev => ({ ...prev, ...result, eventDate: formatDate(result.eventDate) }));
            setCancelReason(result.cancelReason);
    
        }).catch((error) => {
            console.error(error);
        });
    }

    const cancelRequest = async (item) => {

        //console.log(item);
        if (!cancelReason) {
            setNewListError('Cancel Reason is required!!');
            return;
        }

        sponsorClient.cancelRequest(item.id, { id:item.id ,reason: cancelReason }).then(result => {
            //console.log(result);
            //debugger;
            sponsReqDetailsDialogRef.current.close();
            loadRequests();

        }).catch((error) => {
            console.error(error);
        });
    }

    const updateItemDetails = async (item) => {

        sponsReqDetailsDialogRef.current.close();
        newSponsReqDialogRef.current.showModal();
        setIsEditMode(true);
        seteditItemInfo(item);
    }

    const submitRequest = async (item) => {
        try {
            sponsorClient.submitRequest(item.id).then(result => {
                //console.log(result);
                loadRequests();
                sponsReqDetailsDialogRef.current.close();
            });
        }
        catch (e) {
            try {
                const errors = JSON.parse(e.response).errors;
                if (errors?.Title) { setNewListError(errors.Title[0]); return; }
            } catch { /* ignore */ }
            setNewListError('Failed to create list.');
        }

    }


    return (
        <>
            <hgroup>
                <h1>Sponsorship Dashboard</h1>
                <p>Manage your sponsorship</p>
            </hgroup>

            <div className="todo-layout">
                
                    <div className="todo-panel-header">
                        <button
                            className="icon-btn"
                            onClick={showNewSponReqDialog}>
                            <Settings size={40} strokeWidth={2} />
                            Create New Request
                        </button>
                    </div>

                    {sponsorShipRequests && (
                        <table style={{width:'100%'}} >
                            <thead>
                                <tr>
                                    <th>Title</th>
                                    <th>Event</th>
                                    <th>Amount</th>
                                    <th>Status</th>
                                    <th>Created</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                {sponsorShipRequests.map((item, i) =>
                                    <tr key={item.id}>
                                        <td>{item.title}</td>
                                        <td>{item.eventName}</td>
                                        <td>{item.requestedAmount}</td>
                                        <td>{item.status}</td>
                                        <td>{item.createdAt.toISOString().split('T')[0]}</td>
                                        <td>
                                            <button onClick={() => viewSponsorShipDetalils(item)}>View</button>
                                        </td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    )}

            </div>


            {/* New List dialog */}
            <dialog ref={newSponsReqDialogRef}>
                <SponsorshipContext.Provider value={{ newSponsReqDialogRef, isEditMode, loadRequests, editItemInfo }} >
                    <CreateOrUpdateSponsorship />
                </SponsorshipContext.Provider>
            </dialog>



            {/* Sponsor ship Details dialog */}
            <dialog ref={sponsReqDetailsDialogRef}>
                <article>
                    <header>
                        <h3>Item Details</h3>
                        <button rel="prev" aria-label="Close" onClick={() => { sponsReqDetailsDialogRef.current.close(); } }></button>
                    </header>
                    <div className="flex-row">
                        <label htmlFor="requestor" className="header-cell">Requestor</label>
                        <span id="requestor" className="desc-cell" > {requestData.requestorName } </span>
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
                        <span id="financeApprovalRemarks" className="desc-cell"> {requestData.financeApprovalRemarks } </span>
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
                        <label htmlFor="newSponReqEventbusinessBenefit">Cancel Reason *</label>
                        <textarea id="newSponReqEventbusinessBenefit" value={cancelReason} onChange={(e) => { setCancelReason(e.currentTarget.value) }} />
                    </div>

                    {newListError && <small>{newListError}</small>}

                    <footer>
                        <button disabled={!canCancel} className="danger" style={{ marginInlineEnd: 'auto' }} onClick={() => cancelRequest(requestData)}>Cancel Request</button>
                        <button className="secondary" onClick={() => { sponsReqDetailsDialogRef.current.close(); } }>Close</button>
                        <button disabled={!canEdit} onClick={() => updateItemDetails(requestData)}>Update Request</button>
                        <button disabled={!canSubmit} onClick={() => submitRequest(requestData)}>Submit Request</button>
                    </footer>
                </article>
            </dialog>



        </>);
}

export const useSponsorshipContext = () => useContext(SponsorshipContext);