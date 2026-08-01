import React, { useState, useRef, useContext, useEffect } from 'react';
import { Plus, Settings, MoreHorizontal } from 'lucide-react';

import { SponsorshipRequestsClient, SponsorshipTypesClient } from '../web-api-client.ts';

const sponsorClient = new SponsorshipRequestsClient();
const sponsorTypeClient = new SponsorshipTypesClient();

export function Sponsorship() {

    const [sponsorShipRequests, setsponsorShipRequests] = useState([]);
    const [sponsorOptions, setSponsorOptions] = useState([]);

    const [newListError, setNewListError] = useState('');
    const [newSponReqTitle, setNewSponReqTitle] = useState('');
    const [newSponReqDepartment, setNewSponReqDepartment] = useState('');
    const [newSponReqType, setNewSponReqType] = useState('');
    const [newSponReqEventName, setNewSponReqEventName] = useState('');
    const [newSponReqEventDate, setNewSponReqEventDate] = useState(new Date());
    const [newSponReqEventAmount, setNewSponReqEventAmount] = useState(0);
    const [newSponReqEventPurpose, setSewSponReqEventPurpose] = useState('');
    const [newSponReqEventbusinessBenefit, setNewSponReqEventbusinessBenefit] = useState('');
    const [newSponReqsupportingDocumentUrl, setNewSponReqsupportingDocumentUrl] = useState('');


    const newSponsReqDialogRef = useRef(null);

    useEffect(() => {

        sponsorClient.getMyRequests().then(result => {
            //console.log(result.requests);
            setsponsorShipRequests(result.requests); 
            
        }).catch(console.error);

        sponsorTypeClient.getTypes().then(result => {
            //console.log(result.sponsorshipTypes);
            setSponsorOptions(result.sponsorshipTypes);

        }).catch(console.error);

    }, []);

    const showListOptionsDialog = () => {
        // setListOptionsEditor({ id: selectedList.id, title: selectedList.title, colour: selectedList.colour || colours[0].code });
        // listOptionsDialogRef.current.showModal();
    };


    // ── Create requst modal ──────────────────────────────────────────────────────────────────

    const showNewSponReqDialog = () => {
        setNewSponReqTitle('');
        setNewSponReqDepartment('');
        setNewSponReqType('');
        setNewSponReqEventName('');
        setNewSponReqEventDate(new Date());
        setNewSponReqEventAmount(0);
        setSewSponReqEventPurpose('');
        setNewSponReqEventbusinessBenefit('');
        setNewSponReqsupportingDocumentUrl('');
        setNewListError('');
        newSponsReqDialogRef.current.showModal();
        setTimeout(() => document.getElementById('newListTitle')?.focus(), 50);
    };

    const closeNewSponReqDialog = () => {
        newSponsReqDialogRef.current.close();
        setNewSponReqTitle('');
        setNewSponReqDepartment('');
        setNewSponReqType('');
        setNewSponReqEventName('');
        setNewSponReqEventDate(new Date());
        setNewSponReqEventAmount(0);
        setSewSponReqEventPurpose('');
        setNewSponReqEventbusinessBenefit('');
        setNewSponReqsupportingDocumentUrl('');
        setNewListError('');
    };

    const handleDrafRequest = async () => {
        if (!newListTitle.trim()) return;
        try {
            const id = await listsClient.createTodoList({ title: newListTitle.trim(), colour: newListColour });
            const newList = { id, title: newListTitle.trim(), colour: newListColour, items: [] };
            setLists(ls => [...ls, newList]);
            setSelectedListId(id);
            closeNewListDialog();
        } catch (e) {
            try {
                const errors = JSON.parse(e.response).errors;
                if (errors?.Title) { setNewListError(errors.Title[0]); return; }
            } catch { /* ignore */ }
            setNewListError('Failed to create list.');
        }
    };

    const handleSumbitRequest = async () => {
        if (!newListTitle.trim()) return;
        try {
            const id = await listsClient.createTodoList({ title: newListTitle.trim(), colour: newListColour });
            const newList = { id, title: newListTitle.trim(), colour: newListColour, items: [] };
            setLists(ls => [...ls, newList]);
            setSelectedListId(id);
            closeNewListDialog();
        } catch (e) {
            try {
                const errors = JSON.parse(e.response).errors;
                if (errors?.Title) { setNewListError(errors.Title[0]); return; }
            } catch { /* ignore */ }
            setNewListError('Failed to create list.');
        }
    };


    return (
        <>
            <hgroup>
                <h1>Sponsorship Dashboard</h1>
                <p>Manage your sponsorship</p>
            </hgroup>

            <div className="todo-layout">
                <div className="todo-main">
                    <div className="todo-panel-header">
                        <button
                            className="icon-btn"
                            onClick={showNewSponReqDialog}>
                            <Settings size={40} strokeWidth={2} />
                            Create New Request
                        </button>
                    </div>

                    <div className="todo-item-header">
                        <div> Title </div>
                        <div> Event </div>
                        <div> Amount </div>
                        <div> Status </div>
                        <div> Created </div>
                        <div> Action </div>
                    </div>
                    {sponsorShipRequests && (sponsorShipRequests.map((item, i) => (
                        <div key={item.id} className="todo-item">
                            <div className="todo-item-text">{item.title}</div>
                            <div className="todo-item-text">{item.event}</div>
                            <div className="todo-item-text">{item.amount}</div>
                            <div className="todo-item-text">{item.status}</div>
                            <div className="todo-item-text">{item.created}</div>
                            <div className="todo-item-text">
                                <button onClick={() => viewItem(item)}>View</button>
                            </div>
                        </div>
                    )))}
                </div>
            </div>


            {/* New List dialog */}
            <dialog ref={newSponsReqDialogRef}>
                <article>
                    <header>
                        <h3>New List</h3>
                        <button rel="prev" aria-label="Close" onClick={closeNewSponReqDialog}></button>
                    </header>
                    <label htmlFor="newSponReqTitle">Title</label>
                    <input type="text" id="newSponReqTitle" placeholder="Enter title"
                        value={newSponReqTitle} onChange={e => setNewSponReqTitle(e.target.value)}
                        aria-invalid={newListError ? 'true' : undefined}
                        maxLength={200} />

                    <label htmlFor="newSponReqDepartment">Department</label>
                    <input type="text" id="newSponReqDepartment" placeholder="Enter Department"
                        value={newSponReqDepartment} onChange={e => setNewSponReqDepartment(e.target.value)}
                        aria-invalid={newListError ? 'true' : undefined}
                        maxLength={200} />


                    <label htmlFor="newSponReqType">Sponsorship Type</label>
                    <select type="text" id="newSponReqType" placeholder="Enter Department"
                        value={newSponReqType} onChange={e => setNewSponReqType(e.target.value)}
                        aria-invalid={newListError ? 'true' : undefined}
                        maxLength={200} >
                        {sponsorOptions && (sponsorOptions.map((item, index) => (
                            <option key={item.id}>
                                {item.name}
                            </option>
                        )))}
                    </select>


                    <label htmlFor="newSponReqEventName">Event Name</label>
                    <input type="text" id="newSponReqEventName" placeholder="Enter Event Name"
                        value={newSponReqEventName} onChange={e => setNewSponReqEventName(e.target.value)}
                        aria-invalid={newListError ? 'true' : undefined}
                        maxLength={200} />

                    <label htmlFor="newSponReqEventDate">Event Date *</label>
                    <input type="date" id="newSponReqEventDate" placeholder="Enter Event Date"
                        value={newSponReqEventDate.toISOString().split("T")[0]} onChange={e => setNewSponReqEventDate(new Date(e.target.value))}
                        aria-invalid={newListError ? 'true' : undefined}
                        maxLength={200} />


                    <label htmlFor="newSponReqEventAmount">Requested Amount *</label>
                    <input type="number" id="newSponReqEventAmount" placeholder="Enter Amount"
                        value={newSponReqEventAmount} onChange={e => setNewSponReqEventAmount(e.target.value)}
                        aria-invalid={newListError ? 'true' : undefined}
                        maxLength={200} />


                    <label htmlFor="newSponReqEventPurpose">Purpose *</label>
                    <textarea type="text" id="newSponReqEventPurpose" placeholder="Enter Amount"
                        value={newSponReqEventPurpose} onChange={e => setSewSponReqEventPurpose(e.target.value)}
                        aria-invalid={newListError ? 'true' : undefined}
                        minLength={10}
                        maxLength={200} />


                    <label htmlFor="newSponReqEventbusinessBenefit">Purpose *</label>
                    <textarea type="text" id="newSponReqEventbusinessBenefit" placeholder="Enter Amount"
                        value={newSponReqEventbusinessBenefit} onChange={e => setNewSponReqEventbusinessBenefit(e.target.value)}
                        />

                    <label htmlFor="newSponReqEventbusinessBenefit">Purpose *</label>
                    <input type="url" id="newSponReqEventbusinessBenefit" placeholder="Enter Amount"
                        value={newSponReqsupportingDocumentUrl} onChange={e => setNewSponReqsupportingDocumentUrl(e.target.value)}
                    />

                    {newListError && <small>{newListError}</small>}


                    <footer>
                        <button className="secondary" onClick={closeNewSponReqDialog}>Cancel</button>
                        <button onClick={handleDrafRequest}>Save as Draft</button>
                        <button onClick={handleSumbitRequest}>Submit Request</button>
                    </footer>
                </article>
            </dialog>


        </>);
}