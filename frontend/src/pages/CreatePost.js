import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
    Car, Wrench, Film, Cpu, HelpCircle, ArrowLeft,
    MapPin, Clock, Users, Loader2, Send, Tag, ArrowRight, ImagePlus, X
} from 'lucide-react';
import api from '../api';

/* ── Constants ── */

const CATEGORIES = [
    { key: 'COVOITURAGE', label: 'Covoiturage', icon: Car, color: 'blue' },
    { key: 'MATERIAL', label: 'Matériel', icon: Wrench, color: 'orange' },
    { key: 'MEDIAS', label: 'Médias', icon: Film, color: 'purple' },
    { key: 'OUTILS', label: 'Outils', icon: Cpu, color: 'emerald' },
    { key: 'AUTRE', label: 'Autre', icon: HelpCircle, color: 'gray' },
];

const POST_TYPES = [
    { key: 'OFFER', label: 'Je propose', emoji: '🤝' },
    { key: 'DEMAND', label: 'Je recherche', emoji: '🔍' },
];

const COLOR_MAP = {
    blue: { ring: 'ring-blue-500', bg: 'bg-blue-50', border: 'border-blue-300', text: 'text-blue-600' },
    orange: { ring: 'ring-orange-500', bg: 'bg-orange-50', border: 'border-orange-300', text: 'text-orange-600' },
    purple: { ring: 'ring-purple-500', bg: 'bg-purple-50', border: 'border-purple-300', text: 'text-purple-600' },
    emerald: { ring: 'ring-emerald-500', bg: 'bg-emerald-50', border: 'border-emerald-300', text: 'text-emerald-600' },
    gray: { ring: 'ring-gray-500', bg: 'bg-gray-50', border: 'border-gray-300', text: 'text-gray-600' },
};

/* ── CreatePost Page ── */

const CreatePost = () => {
    const navigate = useNavigate();
    const [postType, setPostType] = useState('OFFER');
    const [category, setCategory] = useState('');
    const [title, setTitle] = useState('');
    const [content, setContent] = useState('');
    const [location, setLocation] = useState('');

    // Covoiturage-specific
    const [departureLocation, setDepartureLocation] = useState('');
    const [destinationLocation, setDestinationLocation] = useState('');
    const [departureTime, setDepartureTime] = useState('');
    const [returnTime, setReturnTime] = useState('');
    const [availableSeats, setAvailableSeats] = useState(1);

    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState(null);

    // Image upload
    const [imageFile, setImageFile] = useState(null);
    const [imagePreview, setImagePreview] = useState(null);
    const [uploadingImage, setUploadingImage] = useState(false);

    const isCovoiturage = category === 'COVOITURAGE';

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(null);

        if (!category) {
            setError('Veuillez choisir une catégorie.');
            return;
        }

        setSubmitting(true);

        try {
            if (isCovoiturage) {
                await api.post('/post/coride', {
                    title,
                    content,
                    postType,
                    category,
                    departureLocation,
                    destinationLocation,
                    departureTime: new Date(departureTime).toISOString(),
                    returnTime: returnTime ? new Date(returnTime).toISOString() : null,
                    availableSeats: parseInt(availableSeats, 10),
                });
            } else {
                // Upload image first if present
                let imageUrl = null;
                if (imageFile) {
                    setUploadingImage(true);
                    try {
                        const formData = new FormData();
                        formData.append('file', imageFile);
                        const imgRes = await api.post('/post/upload-image', formData, {
                            headers: { 'Content-Type': 'multipart/form-data' }
                        });
                        imageUrl = imgRes.data.imageUrl;
                    } catch (imgErr) {
                        setError("Échec de l'envoi de l'image.");
                        setSubmitting(false);
                        setUploadingImage(false);
                        return;
                    }
                    setUploadingImage(false);
                }
                await api.post('/post/equipment', {
                    title,
                    content,
                    postType,
                    category,
                    location,
                    imageUrl,
                });
            }
            navigate('/feed');
        } catch (err) {
            console.error('Failed to create post:', err);
            const msg = err.response?.data?.message || err.response?.data?.title || "Échec de la création.";
            setError(msg);
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <div className="min-h-screen bg-gray-50">
            <div className="max-w-2xl mx-auto px-4 py-8">

                {/* Back */}
                <button
                    onClick={() => navigate('/feed')}
                    className="flex items-center gap-1.5 text-sm text-gray-500 hover:text-gray-700 mb-6 transition-colors"
                >
                    <ArrowLeft className="w-4 h-4" />
                    Retour aux annonces
                </button>

                <div className="bg-white rounded-2xl border border-gray-200 shadow-sm overflow-hidden">

                    {/* Header */}
                    <div className="px-6 py-5 border-b border-gray-100">
                        <h1 className="text-xl font-bold text-gray-900">Déposer une annonce</h1>
                        <p className="text-sm text-gray-500 mt-1">Partagez ou recherchez une ressource avec la communauté</p>
                    </div>

                    <form onSubmit={handleSubmit} className="p-6 space-y-6">

                        {/* ── 1. Post Type ── */}
                        <div>
                            <label className="text-sm font-semibold text-gray-700 mb-3 block">Type d'annonce</label>
                            <div className="grid grid-cols-2 gap-3">
                                {POST_TYPES.map(({ key, label, emoji }) => (
                                    <button
                                        key={key}
                                        type="button"
                                        onClick={() => setPostType(key)}
                                        className={`flex items-center justify-center gap-2 px-4 py-3 rounded-xl border-2 text-sm font-medium transition-all
                      ${postType === key
                                                ? 'border-orange-500 bg-orange-50 text-orange-700 ring-2 ring-orange-200'
                                                : 'border-gray-200 bg-white text-gray-600 hover:border-gray-300 hover:bg-gray-50'
                                            }`}
                                    >
                                        <span className="text-lg">{emoji}</span>
                                        {label}
                                    </button>
                                ))}
                            </div>
                        </div>

                        {/* ── 2. Category ── */}
                        <div>
                            <label className="text-sm font-semibold text-gray-700 mb-3 block">Catégorie</label>
                            <div className="grid grid-cols-2 sm:grid-cols-3 gap-2">
                                {CATEGORIES.map(({ key, label, icon: Icon, color }) => {
                                    const active = category === key;
                                    const c = COLOR_MAP[color];
                                    return (
                                        <button
                                            key={key}
                                            type="button"
                                            onClick={() => setCategory(key)}
                                            className={`flex items-center gap-2 px-3 py-2.5 rounded-lg border text-sm font-medium transition-all
                        ${active
                                                    ? `${c.border} ${c.bg} ${c.text} ring-2 ${c.ring}`
                                                    : 'border-gray-200 bg-white text-gray-600 hover:border-gray-300 hover:bg-gray-50'
                                                }`}
                                        >
                                            <Icon className="w-4 h-4" />
                                            {label}
                                        </button>
                                    );
                                })}
                            </div>
                        </div>

                        {/* ── 3. Title ── */}
                        <div>
                            <label htmlFor="title" className="text-sm font-semibold text-gray-700 mb-1.5 block">Titre</label>
                            <input
                                id="title"
                                type="text"
                                value={title}
                                onChange={(e) => setTitle(e.target.value)}
                                placeholder="Ex : Câble HDMI à prêter, Covoiturage Paris → Lyon"
                                required
                                className="w-full px-4 py-2.5 rounded-lg border border-gray-300 focus:outline-none focus:ring-2 focus:ring-orange-400 focus:border-transparent text-sm"
                            />
                        </div>

                        {/* ── 4. Covoiturage-specific fields ── */}
                        {isCovoiturage && (
                            <div className="bg-blue-50 rounded-xl p-5 border border-blue-100 space-y-4">
                                <h3 className="text-sm font-semibold text-blue-700 flex items-center gap-2">
                                    <Car className="w-4 h-4" /> Informations du trajet
                                </h3>

                                {/* Route */}
                                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                                    <div>
                                        <label htmlFor="dep" className="text-xs font-medium text-gray-600 mb-1 block">Départ</label>
                                        <div className="relative">
                                            <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-blue-400" />
                                            <input
                                                id="dep"
                                                type="text"
                                                value={departureLocation}
                                                onChange={(e) => setDepartureLocation(e.target.value)}
                                                placeholder="Ville de départ"
                                                required
                                                className="w-full pl-10 pr-3 py-2.5 rounded-lg border border-blue-200 bg-white focus:outline-none focus:ring-2 focus:ring-blue-400 text-sm"
                                            />
                                        </div>
                                    </div>
                                    <div>
                                        <label htmlFor="dest" className="text-xs font-medium text-gray-600 mb-1 block">Destination</label>
                                        <div className="relative">
                                            <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-blue-400" />
                                            <input
                                                id="dest"
                                                type="text"
                                                value={destinationLocation}
                                                onChange={(e) => setDestinationLocation(e.target.value)}
                                                placeholder="Ville d'arrivée"
                                                required
                                                className="w-full pl-10 pr-3 py-2.5 rounded-lg border border-blue-200 bg-white focus:outline-none focus:ring-2 focus:ring-blue-400 text-sm"
                                            />
                                        </div>
                                    </div>
                                </div>

                                {/* Times */}
                                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                                    <div>
                                        <label htmlFor="depTime" className="text-xs font-medium text-gray-600 mb-1 block">
                                            <Clock className="w-3 h-3 inline mr-1" />Heure de départ
                                        </label>
                                        <input
                                            id="depTime"
                                            type="datetime-local"
                                            value={departureTime}
                                            onChange={(e) => setDepartureTime(e.target.value)}
                                            required
                                            className="w-full px-3 py-2.5 rounded-lg border border-blue-200 bg-white focus:outline-none focus:ring-2 focus:ring-blue-400 text-sm"
                                        />
                                    </div>
                                    <div>
                                        <label htmlFor="retTime" className="text-xs font-medium text-gray-600 mb-1 block">
                                            <Clock className="w-3 h-3 inline mr-1" />Heure de retour (optionnel)
                                        </label>
                                        <input
                                            id="retTime"
                                            type="datetime-local"
                                            value={returnTime}
                                            onChange={(e) => setReturnTime(e.target.value)}
                                            className="w-full px-3 py-2.5 rounded-lg border border-blue-200 bg-white focus:outline-none focus:ring-2 focus:ring-blue-400 text-sm"
                                        />
                                    </div>
                                </div>

                                {/* Seats */}
                                <div>
                                    <label htmlFor="seats" className="text-xs font-medium text-gray-600 mb-1 block">
                                        <Users className="w-3 h-3 inline mr-1" />Places disponibles
                                    </label>
                                    <input
                                        id="seats"
                                        type="number"
                                        min={1}
                                        max={10}
                                        value={availableSeats}
                                        onChange={(e) => setAvailableSeats(e.target.value)}
                                        required
                                        className="w-24 px-3 py-2.5 rounded-lg border border-blue-200 bg-white focus:outline-none focus:ring-2 focus:ring-blue-400 text-sm"
                                    />
                                </div>
                            </div>
                        )}

                        {/* ── 5. Location (non-covoiturage) ── */}
                        {!isCovoiturage && category && (
                            <div>
                                <label htmlFor="location" className="text-sm font-semibold text-gray-700 mb-1.5 block">
                                    Localisation
                                </label>
                                <div className="relative">
                                    <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
                                    <input
                                        id="location"
                                        type="text"
                                        value={location}
                                        onChange={(e) => setLocation(e.target.value)}
                                        placeholder="Campus, bâtiment, ville..."
                                        required
                                        className="w-full pl-10 pr-3 py-2.5 rounded-lg border border-gray-300 focus:outline-none focus:ring-2 focus:ring-orange-400 focus:border-transparent text-sm"
                                    />
                                </div>
                            </div>
                        )}

                        {/* ── 5b. Image Upload (non-covoiturage) ── */}
                        {!isCovoiturage && category && (
                            <div>
                                <label className="text-sm font-semibold text-gray-700 mb-1.5 block">
                                    Image (optionnel)
                                </label>
                                {imagePreview ? (
                                    <div className="relative inline-block">
                                        <img
                                            src={imagePreview}
                                            alt="Aperçu"
                                            className="w-full max-h-48 object-cover rounded-lg border border-gray-200"
                                        />
                                        <button
                                            type="button"
                                            onClick={() => {
                                                setImageFile(null);
                                                setImagePreview(null);
                                            }}
                                            className="absolute top-2 right-2 p-1 bg-white/80 rounded-full hover:bg-white transition-colors"
                                        >
                                            <X className="w-4 h-4 text-gray-600" />
                                        </button>
                                    </div>
                                ) : (
                                    <label className="flex flex-col items-center justify-center w-full h-32 border-2 border-dashed border-gray-300 rounded-lg cursor-pointer hover:border-orange-400 hover:bg-orange-50/50 transition-all">
                                        <ImagePlus className="w-8 h-8 text-gray-400 mb-2" />
                                        <span className="text-sm text-gray-500">Cliquez pour ajouter une image</span>
                                        <input
                                            type="file"
                                            accept="image/*"
                                            className="hidden"
                                            onChange={(e) => {
                                                const file = e.target.files[0];
                                                if (file) {
                                                    setImageFile(file);
                                                    setImagePreview(URL.createObjectURL(file));
                                                }
                                            }}
                                        />
                                    </label>
                                )}
                            </div>
                        )}

                        {/* ── 6. Description ── */}
                        <div>
                            <label htmlFor="content" className="text-sm font-semibold text-gray-700 mb-1.5 block">Description</label>
                            <textarea
                                id="content"
                                value={content}
                                onChange={(e) => setContent(e.target.value)}
                                placeholder={isCovoiturage
                                    ? "Détails du trajet : bagages autorisés, animaux, arrêts possibles..."
                                    : "Décrivez ce que vous proposez ou recherchez..."
                                }
                                rows={4}
                                required
                                className="w-full px-4 py-2.5 rounded-lg border border-gray-300 focus:outline-none focus:ring-2 focus:ring-orange-400 focus:border-transparent text-sm resize-none"
                            />
                        </div>

                        {/* ── Error ── */}
                        {error && (
                            <div className="bg-red-50 border border-red-200 rounded-lg px-4 py-3 text-sm text-red-600">
                                {error}
                            </div>
                        )}

                        {/* ── Submit ── */}
                        <button
                            type="submit"
                            disabled={submitting || !category}
                            className="w-full flex items-center justify-center gap-2 bg-orange-500 hover:bg-orange-600 text-white px-6 py-3 rounded-xl font-semibold text-sm transition-colors shadow-sm disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                            {submitting ? (
                                <Loader2 className="w-5 h-5 animate-spin" />
                            ) : (
                                <>
                                    <Send className="w-4 h-4" />
                                    Publier l'annonce
                                </>
                            )}
                        </button>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default CreatePost;
