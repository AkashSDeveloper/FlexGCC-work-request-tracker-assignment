import {useCallback,useEffect,useState} from 'react';
const statuses=['New','InProgress','Blocked','Completed'],priorities=['Low','Medium','High'];
const empty={title:'',clientName:'',description:'',priority:'Medium',status:'New',dueDate:''};
async function api(path,options={}){const r=await fetch('/api/work-requests'+path,{...options,headers:{'Content-Type':'application/json',...(options.headers||{})}});const body=await r.json().catch(()=>null);if(!r.ok)throw Error(body?.error?.message||`Request failed (${r.status})`);return body;}
export default function App(){
 const [items,setItems]=useState([]),[status,setStatus]=useState(''),[search,setSearch]=useState(''),[page,setPage]=useState(1),[total,setTotal]=useState(0),[loading,setLoading]=useState(false),[error,setError]=useState(''),[form,setForm]=useState(empty),[creating,setCreating]=useState(false),[notes,setNotes]=useState({});
 const load=useCallback(async()=>{setLoading(true);setError('');try{const p=new URLSearchParams({page:String(page),pageSize:'10'});if(status)p.set('status',status);if(search.trim())p.set('search',search.trim());const d=await api('?'+p);setItems(d.items);setTotal(d.total);}catch(e){setError(e.message);}finally{setLoading(false);}},[page,status,search]);
 useEffect(()=>{load();},[load]);
 async function create(e){e.preventDefault();setError('');try{await api('',{method:'POST',body:JSON.stringify({...form,dueDate:new Date(form.dueDate+'T12:00:00Z').toISOString()})});setForm(empty);setCreating(false);setPage(1);await load();}catch(e){setError(e.message);}}
 async function update(id,next){setError('');try{await api(`/${id}/status`,{method:'PATCH',body:JSON.stringify({status:next})});await load();}catch(e){setError(e.message);}}
 async function addNote(id){if(!(notes[id]||'').trim())return;setError('');try{await api(`/${id}/notes`,{method:'POST',body:JSON.stringify({note:notes[id]})});setNotes({...notes,[id]:''});await load();}catch(e){setError(e.message);}}
 const pages=Math.max(1,Math.ceil(total/10));
 return <main className="wrap"><header><div><small>FLEXGCC · PRACTICAL ASSIGNMENT</small><h1>Work Request Tracker</h1><p>Track client requests from intake to completion.</p></div><button onClick={()=>setCreating(!creating)}>{creating?'Cancel':'+ New request'}</button></header>
 {error&&<div className="error" role="alert">{error} <button className="light" onClick={load}>Retry</button></div>}
 {creating&&<section><h2>Create work request</h2><form className="form" onSubmit={create}>
 <label>Title *<input required maxLength="100" value={form.title} onChange={e=>setForm({...form,title:e.target.value})}/></label>
 <label>Client name *<input required maxLength="120" value={form.clientName} onChange={e=>setForm({...form,clientName:e.target.value})}/></label>
 <label>Priority<select value={form.priority} onChange={e=>setForm({...form,priority:e.target.value})}>{priorities.map(x=><option key={x}>{x}</option>)}</select></label>
 <label>Status<select value={form.status} onChange={e=>setForm({...form,status:e.target.value})}>{statuses.map(x=><option key={x}>{x}</option>)}</select></label>
 <label>Due date *<input type="date" required value={form.dueDate} onChange={e=>setForm({...form,dueDate:e.target.value})}/></label>
 <label className="wide">Description<textarea maxLength="2000" value={form.description} onChange={e=>setForm({...form,description:e.target.value})}/></label>
 <button className="wide">Create request</button></form></section>}
 <section><div className="bar"><h2>Work requests <span>{total}</span></h2><div className="filters"><input placeholder="Search title or client…" value={search} onChange={e=>{setSearch(e.target.value);setPage(1);}}/><select value={status} onChange={e=>{setStatus(e.target.value);setPage(1);}}><option value="">All statuses</option>{statuses.map(x=><option key={x}>{x}</option>)}</select></div></div>
 {loading?<p className="empty">Loading requests…</p>:items.length===0?<p className="empty">No requests found. Change the filters or create one.</p>:items.map(x=><article key={x.id}><div className="top"><div><small>{x.clientName}</small><h3>{x.title}</h3><p>{x.description||'No description provided.'}</p></div><b className={'priority '+x.priority.toLowerCase()}>{x.priority}</b></div>
 <div className="meta"><span>Due {new Date(x.dueDate).toLocaleDateString()}</span><span>Created {new Date(x.createdDate).toLocaleDateString()}</span><label>Status <select value={x.status} onChange={e=>update(x.id,e.target.value)}>{statuses.map(s=><option key={s}>{s}</option>)}</select></label></div>
 {x.notes&&<p className="notes"><b>Notes</b><br/>{x.notes}</p>}<div className="note"><input placeholder="Add a note…" value={notes[x.id]||''} onChange={e=>setNotes({...notes,[x.id]:e.target.value})}/><button className="light" onClick={()=>addNote(x.id)}>Add note</button></div></article>)}
 <div className="pagination"><span>Page {page} of {pages}</span><div><button className="light" disabled={page<=1} onClick={()=>setPage(page-1)}>Previous</button> <button className="light" disabled={page>=pages} onClick={()=>setPage(page+1)}>Next</button></div></div></section><footer>ASP.NET Core Web API · React · SQLite</footer></main>;
}
