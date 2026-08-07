import React, { useState, useRef, useContext, useEffect } from 'react';
import { Plus, Settings, MoreHorizontal } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';

import { SponsorshipRequestsClient, SponsorshipTypesClient } from '../../web-api-client.ts';
import { useSponsorshipContext } from './Sponsorship.jsx';

const sponsorClient = new SponsorshipRequestsClient();
const sponsorTypeClient = new SponsorshipTypesClient();

const sponsorshipSchema = z.object({
    title: z.string().min(1, 'Title is required').max(200),
    department: z.string().max(200).optional(),
    sponsorshipTypeId: z.string().min(1, 'Select a sponsorship type'),
    eventName: z.string().min(1, 'Event name is required'),
    eventDate: z.string().refine(s => !!s && !Number.isNaN(Date.parse(s)), { message: 'Valid date required' }),
    requestedAmount: z.preprocess(v => Number(v), z.number().min(1, 'Amount must be > 0')),
    purpose: z.string().min(10, 'Purpose must be at least 10 chars').max(200),
    //businessBenefit: z.string().min(10, 'Business benefit must be at least 10 chars').max(200),
    //supportingDocumentUrl: z.string().url().optional().or(z.literal('')),
});

export function CreateOrUpdateSponsorship() {

    const [sponsorOptions, setSponsorOptions] = useState([]);

    const [newListError, setNewListError] = useState('');

    const { newSponsReqDialogRef, isEditMode, loadRequests, editItemInfo } = useSponsorshipContext();

    const [isDraftSubmitting, setIsDraftSubmitting] = useState(false);

    const { register, handleSubmit, formState: { errors, isValid, isSubmitting }, reset, getValues, trigger } = useForm({
        resolver: zodResolver(sponsorshipSchema),
        mode: "onChange",
        defaultValues: {
            title: '',
            department: '',
            sponsorshipTypeId: '',
            eventName: '',
            eventDate: new Date().toISOString().split('T')[0],
            requestedAmount: '',
            purpose: '',
            businessBenefit: '',
            supportingDocumentUrl: '',
        }
    });

    useEffect(() => {

        sponsorTypeClient.getTypes().then(result => {
            //console.log(result.sponsorshipTypes);
            setSponsorOptions(result.sponsorshipTypes);

        }).catch(console.error);

    }, []);

    useEffect(() => {

        if (isEditMode) {
            reset({ ...editItemInfo, sponsorshipTypeId:editItemInfo.sponsorshipType });
        }

    }, [isEditMode, editItemInfo]);

    const closeNewSponReqDialog = () => {
        newSponsReqDialogRef.current.close();
    };


    const handleDrafRequest = async (e) => {
        e?.preventDefault();
        const valid = await trigger(); // runs same validation as submit
        if (!valid) {
            // validation failed — errors object populated, you can focus first error if desired
            const firstErrorField = Object.keys(errors)[0];
            document.getElementById(`newSponReq${capitalize(firstErrorField)}`)?.focus();
            return;
        }
        const values = getValues();
        const payload = {
            ...values,
            eventDate: new Date(values.eventDate),
            requestedAmount: Number(values.requestedAmount)
        };
        setIsDraftSubmitting(true);

        if (isEditMode) {
            payload.id = editItemInfo.id;
            sponsorClient.updateRequest(editItemInfo.id, payload).then(result => {
                reset();
                setIsDraftSubmitting(false);
                closeNewSponReqDialog();
                if (typeof loadRequests === 'function')
                    loadRequests();

            }).catch((error) => {
                console.error(error);
                setIsDraftSubmitting(false);
            });
        }
        else {
            sponsorClient.createRequest(payload).then(result => {
                console.log(result);
                reset();
                setIsDraftSubmitting(false);
                closeNewSponReqDialog();
                if (typeof loadRequests === 'function')
                    loadRequests();

            }).catch((error) => {
                console.error(error);
                setIsDraftSubmitting(false);
            });
        }

        // optionally close/reset
        // closeNewSponReqDialog();
        // reset();
    };

    const handleSumbitRequest = async (values) => {

        // values validated and typed by schema
        const payload = {
            ...values,
            eventDate: new Date(values.eventDate).toISOString(),
            requestedAmount: Number(values.requestedAmount),
        };

        if (isEditMode) {

            try {
                payload.id = editItemInfo.id;
                sponsorClient.updateRequest(editItemInfo.id, payload).then(result => {
                    sponsorClient.submitRequest(editItemInfo.id).then(result => {
                        reset();
                        closeNewSponReqDialog();

                        if (typeof loadRequests === 'function')
                            loadRequests();
                    });
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
        else
        {
            try {

                sponsorClient.createRequest(payload).then(result => {
                    console.log(result);
                    sponsorClient.submitRequest(result).then(result => {
                        console.log(result);
                        reset();
                        closeNewSponReqDialog();

                        if (typeof loadRequests === 'function')
                            loadRequests();
                    });
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
    };



    return (
        <>
            <form onSubmit={handleSubmit(handleSumbitRequest)}>
                <article>
                    <header>
                        <h3>New List</h3>
                        <button type="button" rel="prev" aria-label="Close" onClick={closeNewSponReqDialog}></button>
                    </header>

                    <label htmlFor="newSponReqTitle">Title</label>
                    <input id="newSponReqTitle" {...register('title')} />
                    {errors.title && <small role="alert">{errors.title.message}</small>}

                    <label htmlFor="newSponReqDepartment">Department</label>
                    <input id="newSponReqDepartment" {...register('department')} />
                    {errors.department && <small role="alert">{errors.department.message}</small>}

                    <label htmlFor="newSponReqType">Sponsorship Type</label>
                    <select id="newSponReqType" {...register('sponsorshipTypeId')}>
                        <option value="">-- choose --</option>
                        {sponsorOptions.map(opt => <option key={opt.id} value={opt.id}>{opt.name}</option>)}
                    </select>
                    {errors.type && <small role="alert">{errors.type.message}</small>}

                    <label htmlFor="newSponReqEventName">Event Name</label>
                    <input id="newSponReqEventName" {...register('eventName')} />
                    {errors.eventName && <small role="alert">{errors.eventName.message}</small>}

                    <label htmlFor="newSponReqEventDate">Event Date</label>
                    <input id="newSponReqEventDate" type="date" {...register('eventDate')} />
                    {errors.eventDate && <small role="alert">{errors.eventDate.message}</small>}

                    <label htmlFor="newSponReqEventAmount">Requested Amount</label>
                    <input id="newSponReqEventAmount" type="number" {...register('requestedAmount')} />
                    {errors.requestedAmount && <small role="alert">{errors.requestedAmount.message}</small>}

                    <label htmlFor="newSponReqEventPurpose">Purpose</label>
                    <textarea id="newSponReqEventPurpose" {...register('purpose')} />
                    {errors.purpose && <small role="alert">{errors.purpose.message}</small>}

                    <label htmlFor="newSponReqEventbusinessBenefit">Business Benefit</label>
                    <textarea id="newSponReqEventbusinessBenefit" {...register('businessBenefit')} />
                    {errors.businessBenefit && <small role="alert">{errors.businessBenefit.message}</small>}

                    <label htmlFor="newSponReqsupportingDocumentUrl">Supporting Document URL</label>
                    <input id="newSponReqsupportingDocumentUrl" type="url" {...register('supportingDocumentUrl')} />
                    {errors.supportingDocumentUrl && <small role="alert">{errors.supportingDocumentUrl.message}</small>}

                    {newListError && <small>{newListError}</small>}
                    <footer>
                        <button className="secondary" onClick={closeNewSponReqDialog}>Cancel</button>
                        <button onClick={async (e) => handleDrafRequest(e)} disabled={!isValid || isDraftSubmitting} style={{ width: '150px !important' }} >Save as Draft</button>
                        <button type="submit" disabled={!isValid || isSubmitting}  >Submit Request</button>
                    </footer>
                </article>
            </form>
        </>
    );
}